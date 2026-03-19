# CQRS Pattern Deep Dive

Understanding how Command Query Responsibility Segregation is implemented in this project.

## Why CQRS?

Traditional architectures use the same model for reading and writing data. CQRS separates these concerns for:

- **Independent scaling** of read and write operations
- **Optimized data models** for each operation type
- **Clearer intent** in code (is this a read or write?)
- **Easier testing** of business logic in isolation

## Implementation Overview

```
┌──────────────────────────────────────────────────────────────┐
│                         Endpoint                             │
│                 (GamesEndpoints.cs)                          │
└──────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌──────────────────────────────────────────────────────────────┐
│                         MediatR                               │
│                  (In-Process Messenger)                      │
└──────────────────────────────────────────────────────────────┘
                              │
          ┌───────────────────┴───────────────────┐
          ▼                                       ▼
┌─────────────────────┐                 ┌─────────────────────┐
│      Commands       │                 │       Queries        │
│   (Write/Modify)    │                 │     (Read-Only)      │
└─────────────────────┘                 └─────────────────────┘
          │                                       │
          ▼                                       ▼
┌─────────────────────┐                 ┌─────────────────────┐
│   Command Handlers  │                 │   Query Handlers     │
│ (CreateGameHandler) │                 │ (GetAllGamesHandler) │
└─────────────────────┘                 └─────────────────────┘
          │                                       │
          ▼                                       ▼
┌──────────────────────────────────────────────────────────────┐
│                     IGameRepository                          │
│              (Single Source of Truth)                        │
└──────────────────────────────────────────────────────────────┘
```

## Commands

### Command Structure

Commands are **requests to change state**. They should:

- Be named with verb + entity + "Command" (e.g., `CreateGameCommand`)
- Return the affected entity or a result type
- Be immutable records

### Example: CreateGameCommand

```csharp
// Application/Commands/CreateGameCommand.cs
namespace GameStore.Api.Application.Commands;

public record CreateGameCommand(
    string Name,
    int GenreId,
    decimal Price,
    DateOnly ReleaseDate
) : IRequest<GameDto>;
```

### Command Handler

```csharp
// Application/Handlers/CreateGameCommandHandler.cs
namespace GameStore.Api.Application.Handlers;

public class CreateGameCommandHandler(
    IGameRepository repository,
    IGameQueryService queryService)
    : IRequestHandler<CreateGameCommand, GameDto>
{
    public async Task<GameDto> Handle(
        CreateGameCommand request,
        CancellationToken ct)
    {
        var genre = await repository.GetGenreByIdAsync(request.GenreId, ct)
            ?? throw new InvalidOperationException($"Genre {request.GenreId} not found");

        var game = new Game
        {
            Name = request.Name,
            GenreId = request.GenreId,
            Price = request.Price,
            ReleaseDate = request.ReleaseDate
        };

        var created = await repository.CreateAsync(game, ct);

        return new GameDto(
            created.Id,
            created.Name,
            genre.Name,
            created.Price,
            created.ReleaseDate);
    }
}
```

### Registering Commands

MediatR automatically discovers handlers from the assembly:

```csharp
// Program.cs
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(CreateGameCommand).Assembly));
```

## Queries

### Query Structure

Queries are **requests for data**. They should:

- Be named with verb + entity + "Query" (e.g., `GetAllGamesQuery`)
- Return DTOs (not entities)
- Be immutable records

### Example: GetAllGamesQuery

```csharp
// Application/Queries/GetAllGamesQuery.cs
namespace GameStore.Api.Application.Queries;

public record GetAllGamesQuery : IRequest<List<GameDto>>;
```

### Query Handler with Cache

```csharp
// Application/Handlers/GetAllGamesQueryHandler.cs
namespace GameStore.Api.Application.Handlers;

public class GetAllGamesQueryHandler(
    IGameQueryService queryService)
    : IRequestHandler<GetAllGamesQuery, List<GameDto>>
{
    private readonly IGameQueryService _queryService = queryService;

    public async Task<List<GameDto>> Handle(
        GetAllGamesQuery request,
        CancellationToken ct)
    {
        return await _queryService.GetAllGamesAsync();
    }
}
```

## DTOs vs Entities

### Entities (Domain)

```csharp
// Domain/Game.cs
public class Game
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public Genre? Genre { get; set; }
    public int GenreId { get; set; }
    public decimal Price { get; set; }
    public DateOnly ReleaseDate { get; set; }
}
```

### DTOs (Application Layer)

```csharp
// Application/GameDtos.cs
public record GameDto(
    int Id,
    string Name,
    string Genre,
    decimal Price,
    DateOnly ReleaseDate);
```

**Key Differences:**

| Aspect | Entity | DTO |
|--------|--------|-----|
| Purpose | Domain model | API contract |
| Relations | Include navigation | Flattened/denormalized |
| Validation | Domain rules | API validation |
| Persistence | Mapped to DB | Not mapped |

## Caching Layer

### Why Cache?

Query handlers delegate to `IGameQueryService` which:
- Checks cache first (Redis or in-memory)
- Falls back to repository if cache miss
- Returns consistent DTO format

### Cache Interface

```csharp
// Application/Cache/IGameQueryService.cs
public interface IGameQueryService
{
    Task<List<GameDto>> GetAllGamesAsync();
    Task<GameDto?> GetGameByIdAsync(int id);
    Task InvalidateCacheAsync(int? gameId = null);
}
```

### Cache Implementation

```csharp
// Application/Cache/RedisGameQueryService.cs
public class RedisGameQueryService : IGameQueryService
{
    private readonly IDistributedCache _cache;
    private readonly IGameRepository _repository;

    public async Task<List<GameDto>> GetAllGamesAsync()
    {
        var cached = await _cache.GetStringAsync("games:all");
        if (cached is not null)
            return JsonSerializer.Deserialize<List<GameDto>>(cached)!;

        var games = await _repository.GetAllAsync();
        var dtos = games.Select(MapToDto).ToList();

        await _cache.SetStringAsync("games:all", JsonSerializer.Serialize(dtos));
        return dtos;
    }
}
```

## Best Practices

### 1. Keep Commands Small

```csharp
// Good: Focused command
public record UpdateGamePriceCommand(int GameId, decimal NewPrice) : IRequest<bool>;

// Avoid: God command
public record UpdateGameCommand(string Name, int GenreId, decimal Price, 
    DateOnly ReleaseDate, string Description, string Developer, ...) 
    : IRequest<GameDto>;
```

### 2. Use Result Types for Errors

```csharp
public record UpdateGameCommand(...) : IRequest<Result<GameDto>>;

public class UpdateGameCommandHandler : IRequestHandler<UpdateGameCommand, Result<GameDto>>
{
    public async Task<Result<GameDto>> Handle(UpdateGameCommand request, CancellationToken ct)
    {
        var game = await repository.GetByIdAsync(request.GameId, ct);
        if (game is null)
            return Result<GameDto>.Failure("Game not found");
        
        // Update logic...
        return Result<GameDto>.Success(dto);
    }
}
```

### 3. Validate in Handlers

```csharp
public async Task<GameDto> Handle(CreateGameCommand request, CancellationToken ct)
{
    if (string.IsNullOrWhiteSpace(request.Name))
        throw new ArgumentException("Name is required", nameof(request));
    
    if (request.Price < 0)
        throw new ArgumentException("Price cannot be negative", nameof(request));
    
    // Continue...
}
```

### 4. Use Cancellation Tokens

```csharp
// Always accept and pass cancellation tokens
public async Task<GameDto> Handle(GetGameByIdQuery request, CancellationToken ct)
{
    return await repository.GetByIdAsync(request.Id, ct)
        ?? throw new KeyNotFoundException($"Game {request.Id} not found");
}
```

## MediatR Pipeline

### Logging Behavior

Add logging to all requests:

```csharp
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateGameCommand).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
});
```

### Validation Behavior

Add validation pipeline:

```csharp
cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
```

## Testing Commands and Queries

### Test Command Handler

```csharp
[Fact]
public async Task CreateGameCommand_ReturnsGameDto()
{
    // Arrange
    var repository = new Mock<IGameRepository>();
    var queryService = new Mock<IGameQueryService>();
    var handler = new CreateGameCommandHandler(
        repository.Object, 
        queryService.Object);
    
    var command = new CreateGameCommand("New Game", 1, 49.99m, DateOnly.FromDateTime(DateTime.Now));
    
    repository.Setup(r => r.GetGenreByIdAsync(1, default))
        .ReturnsAsync(new Genre { Id = 1, Name = "Action" });
    
    repository.Setup(r => r.CreateAsync(It.IsAny<Game>(), default))
        .ReturnsAsync((Game g, CancellationToken _) => { g.Id = 1; return g; });
    
    // Act
    var result = await handler.Handle(command, default);
    
    // Assert
    Assert.Equal("New Game", result.Name);
    Assert.Equal("Action", result.Genre);
}
```

### Test Query Handler

```csharp
[Fact]
public async Task GetAllGamesQuery_ReturnsCachedData()
{
    // Arrange
    var queryService = new Mock<IGameQueryService>();
    var handler = new GetAllGamesQueryHandler(queryService.Object);
    
    var expected = new List<GameDto>
    {
        new(1, "Game 1", "Action", 29.99m, DateOnly.FromDateTime(DateTime.Now))
    };
    
    queryService.Setup(q => q.GetAllGamesAsync())
        .ReturnsAsync(expected);
    
    // Act
    var result = await handler.Handle(new GetAllGamesQuery(), default);
    
    // Assert
    Assert.Single(result);
    Assert.Equal("Game 1", result[0].Name);
}
```

## Summary

| Aspect | Command | Query |
|--------|---------|-------|
| Purpose | Modify state | Read state |
| Returns | Entity/DTO/Result | DTO/Result |
| Caching | No (writes invalidate) | Yes |
| Handler suffix | `CommandHandler` | `QueryHandler` |
| Interface | `IRequest<T>` | `IRequest<T>` |

## Next Steps

- [Architecture Overview](./README.md) - High-level architecture
- [API Reference](../api/README.md) - Endpoint documentation
- [Testing Guide](../../GameStore.Api.test/docs/README.md) - Test patterns
