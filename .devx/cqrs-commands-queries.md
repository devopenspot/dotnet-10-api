# Commands & Queries

This document details the CQRS implementation using MediatR.

## Overview

Commands handle **write operations** (Create, Update, Delete).
Queries handle **read operations** (Get).

## Command Pattern

### Create Game

```csharp
public record CreateGameCommand(CreateGameDto Dto) : IRequest<GameDto>;
```

**Handler:**
```csharp
public class CreateGameHandler : IRequestHandler<CreateGameCommand, GameDto>
{
    private readonly IGameRepository _repository;
    
    public CreateGameHandler(IGameRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<GameDto> Handle(CreateGameCommand request, CancellationToken ct)
    {
        var game = new Game
        {
            Name = request.Dto.Name,
            GenreId = request.Dto.GenreId,
            Price = request.Dto.Price,
            ReleaseDate = request.Dto.ReleaseDate
        };
        
        var created = await _repository.CreateAsync(game, ct);
        return MapToDto(created);
    }
}
```

### Update Game

```csharp
public record UpdateGameCommand(int Id, UpdateGameDto Dto) : IRequest<GameDto?>;
```

### Delete Game

```csharp
public record DeleteGameCommand(int Id) : IRequest<bool>;
```

## Query Pattern

### Get All Games

```csharp
public record GetAllGamesQuery : IRequest<IEnumerable<GameDto>>;
```

**Handler:**
```csharp
public class GetAllGamesHandler : IRequestHandler<GetAllGamesQuery, IEnumerable<GameDto>>
{
    private readonly IGameRepository _repository;
    
    public GetAllGamesHandler(IGameRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<IEnumerable<GameDto>> Handle(GetAllGamesQuery request, CancellationToken ct)
    {
        var games = await _repository.GetAllAsync(ct);
        return games.Select(MapToDto);
    }
}
```

### Get Game by ID

```csharp
public record GetGameByIdQuery(int Id) : IRequest<GameDto?>;
```

## Cancellation Tokens

Always accept and pass `CancellationToken`:

```csharp
public interface IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
```

## Best Practices

1. **Commands return the affected entity** (or DTO)
2. **Queries should be read-only** - no side effects
3. **Use records for commands/queries** - immutability
4. **Keep handlers focused** - single responsibility
5. **Use CancellationToken** - proper cancellation support
