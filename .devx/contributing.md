# Contributing

## Development Workflow

### 1. Branch Strategy

```bash
git checkout -b feature/add-rating-system
git checkout -b fix/delete-endpoint-validation
git checkout -b docs/update-api-documentation
```

**Branch naming:**
- `feature/` - New features
- `fix/` - Bug fixes
- `docs/` - Documentation
- `refactor/` - Code improvements

### 2. Making Changes

1. Write code following our conventions
2. Add/update tests
3. Run the test suite
4. Submit a pull request

### 3. Commit Messages

```
feat: add game rating system
fix: resolve null reference in GetGameById
docs: update API documentation
refactor: extract repository interface
test: add integration tests for endpoints
```

## Code Conventions

### Naming

| Element | Convention | Example |
|---------|------------|---------|
| Classes | PascalCase | `GameRepository` |
| Methods | PascalCase | `GetAllGamesAsync` |
| Properties | PascalCase | `ReleaseDate` |
| Parameters | camelCase | `gameId` |
| Private fields | _camelCase | `_context` |
| Interfaces | IPascalCase | `IGameRepository` |
| Commands | PascalCase with suffix | `CreateGameCommand` |
| Queries | PascalCase with suffix | `GetAllGamesQuery` |

### File Organization

```
Application/
├── Commands/
│   └── Game/
│       ├── CreateGameCommand.cs
│       └── CreateGameCommandHandler.cs
├── Queries/
│   └── Game/
│       ├── GetAllGamesQuery.cs
│       └── GetAllGamesQueryHandler.cs
├── Dtos/
│   └── GameDto.cs
└── Ports/
    └── IGameRepository.cs
```

### Adding New Features

#### 1. Domain Entity

```csharp
// Domain/Rating.cs
public class Rating
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public Game Game { get; set; } = null!;
    public int Score { get; set; }
    public string UserId { get; set; } = string.Empty;
}
```

#### 2. DTO

```csharp
// Application/Dtos/CreateRatingDto.cs
public record CreateRatingDto(
    int GameId,
    int Score,
    string UserId
);
```

#### 3. Command/Query

```csharp
// Application/Commands/Rating/CreateRatingCommand.cs
public record CreateRatingCommand(CreateRatingDto Dto) : IRequest<RatingDto>;
```

#### 4. Handler

```csharp
// Application/Handlers/Rating/CreateRatingHandler.cs
public class CreateRatingHandler : IRequestHandler<CreateRatingCommand, RatingDto>
{
    private readonly IRatingRepository _repository;
    
    public CreateRatingHandler(IRatingRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<RatingDto> Handle(CreateRatingCommand request, CancellationToken ct)
    {
        var rating = new Rating
        {
            GameId = request.Dto.GameId,
            Score = request.Dto.Score,
            UserId = request.Dto.UserId
        };
        
        var created = await _repository.CreateAsync(rating, ct);
        return new RatingDto(created.Id, created.GameId, created.Score, created.UserId);
    }
}
```

#### 5. Endpoint

```csharp
// Endpoints/RatingEndpoints.cs
public static class RatingEndpoints
{
    public static void MapRatingEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/ratings").WithTags("Ratings");
        
        group.MapPost("/", async (CreateRatingDto dto, IMediator mediator) =>
        {
            var command = new CreateRatingCommand(dto);
            var result = await mediator.Send(command);
            return Results.Created($"/ratings/{result.Id}", result);
        });
    }
}
```

## Pull Request Checklist

- [ ] Code follows naming conventions
- [ ] Tests added/updated
- [ ] `dotnet build` succeeds
- [ ] `dotnet test` passes
- [ ] Documentation updated
- [ ] Commit messages are clear
