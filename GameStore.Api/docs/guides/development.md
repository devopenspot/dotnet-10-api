# Development Guides

Practical guides for common development tasks.

## Table of Contents

- [Creating a New Migration](#creating-a-new-migration)
- [Adding a New Endpoint](#adding-a-new-endpoint)
- [Working with Commands](#working-with-commands)
- [Working with Queries](#working-with-queries)

---

## Creating a New Migration

When you modify domain entities, create a migration to update the database schema.

### Step 1: Add Entity Changes

Modify your entity in `Domain/`:

```csharp
public class Game
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    // Add new property
    public string? Description { get; set; }  // NEW
}
```

### Step 2: Create Migration

```bash
cd GameStore.Api
dotnet ef migrations add AddGameDescription
```

This creates a migration file in `Infrastructure/Migrations/`.

### Step 3: Apply Migration

Migrations run automatically on startup via `app.MigrateDatabase()` in `Program.cs`.

To apply manually:

```bash
dotnet ef database update
```

### Migration File Structure

```
Infrastructure/Migrations/
├── 20260314013929_InitialCreate.cs
├── 20260314013929_InitialCreate.Designer.cs
└── GameStoreContextModelSnapshot.cs
```

### Revert Migration

```bash
dotnet ef migrations remove    # Remove last migration (if not applied)
dotnet ef database update <previous-migration>  # Rollback
```

---

## Adding a New Endpoint

### Example: Add a Genre Endpoint

#### Step 1: Create Command (if needed)

```csharp
// Application/Commands/CreateGenreCommand.cs
namespace GameStore.Api.Application.Commands;

public record CreateGenreCommand(string Name) : IRequest<Genre>;
```

#### Step 2: Create Handler

```csharp
// Application/Handlers/CreateGenreCommandHandler.cs
namespace GameStore.Api.Application.Handlers;

public class CreateGenreCommandHandler(
    IGenreRepository repository) 
    : IRequestHandler<CreateGenreCommand, Genre>
{
    public async Task<Genre> Handle(
        CreateGenreCommand request, 
        CancellationToken ct)
    {
        var genre = new Genre { Name = request.Name };
        return await repository.CreateAsync(genre, ct);
    }
}
```

#### Step 3: Create DTO

```csharp
// Application/GenreDtos.cs
namespace GameStore.Api.Application;

public record GenreDto(int Id, string Name);
```

#### Step 4: Add Endpoint

```csharp
// Endpoints/GenresEndpoints.cs
public static class GenresEndpoints
{
    public static void MapGenresEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/genres");

        group.MapPost("/", async (CreateGenreCommand command, IMediator mediator) =>
        {
            var genre = await mediator.Send(command);
            return Results.Created($"/genres/{genre.Id}", new GenreDto(genre.Id, genre.Name));
        });
    }
}
```

#### Step 5: Register Endpoint

In `Program.cs`:

```csharp
app.MapGenresEndpoints();
```

---

## Working with Commands

### Command Pattern

Commands represent **write operations** that modify state.

### Creating a Command

```csharp
// Application/Commands/AddRatingCommand.cs
public record AddRatingCommand(int GameId, int Rating) : IRequest<bool>;
```

### Handling a Command

```csharp
// Application/Handlers/AddRatingCommandHandler.cs
public class AddRatingCommandHandler(
    IGameRepository repository) 
    : IRequestHandler<AddRatingCommand, bool>
{
    public async Task<bool> Handle(
        AddRatingCommand request, 
        CancellationToken ct)
    {
        var game = await repository.GetByIdAsync(request.GameId, ct);
        if (game is null) return false;
        
        // Business logic here
        game.Rating = request.Rating;
        await repository.UpdateAsync(game, ct);
        return true;
    }
}
```

### Dispatching Commands

From an endpoint:

```csharp
group.MapPost("/{id}/rating", async (int id, int rating, IMediator mediator) =>
{
    var result = await mediator.Send(new AddRatingCommand(id, rating));
    return result ? Results.Ok() : Results.NotFound();
});
```

---

## Working with Queries

### Query Pattern

Queries represent **read operations** that don't modify state.

### Creating a Query

```csharp
// Application/Queries/GetGamesByGenreQuery.cs
public record GetGamesByGenreQuery(int GenreId) : IRequest<List<GameDto>>;
```

### Handling a Query

```csharp
// Application/Handlers/GetGamesByGenreQueryHandler.cs
public class GetGamesByGenreQueryHandler(
    IGameRepository repository,
    IGameQueryService queryService) 
    : IRequestHandler<GetGamesByGenreQuery, List<GameDto>>
{
    public async Task<List<GameDto>> Handle(
        GetGamesByGenreQuery request, 
        CancellationToken ct)
    {
        var games = await repository.GetAllAsync(ct);
        return games
            .Where(g => g.GenreId == request.GenreId)
            .Select(g => new GameDto(g.Id, g.Name, g.Genre!.Name, g.Price, g.ReleaseDate))
            .ToList();
    }
}
```

### Using Cache

For cached queries:

```csharp
public class GetGamesByGenreQueryHandler(
    IGameQueryService queryService) 
    : IRequestHandler<GetGamesByGenreQuery, List<GameDto>>
{
    public async Task<List<GameDto>> Handle(
        GetGamesByGenreQuery request, 
        CancellationToken ct)
    {
        // Uses Redis or in-memory cache
        var games = await queryService.GetGamesByGenreAsync(request.GenreId);
        return games;
    }
}
```

---

## Testing Your Changes

Always test new functionality:

```bash
cd GameStore.Api.test
dotnet test
```

See [Testing Guide](../GameStore.Api.test/docs/README.md) for detailed testing instructions.
