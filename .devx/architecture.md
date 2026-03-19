# Architecture

## Overview

GameStore.Api follows **Clean Architecture** principles with **CQRS** pattern implementation.

## Layer Structure

```
┌─────────────────────────────────────────────────┐
│                   Endpoints                      │
│            (Minimal API Routes)                  │
├─────────────────────────────────────────────────┤
│                  Application                      │
│    Commands │ Queries │ Handlers │ DTOs │ Ports │
├─────────────────────────────────────────────────┤
│                   Infrastructure                  │
│        EF Core │ Repositories │ Migrations        │
├─────────────────────────────────────────────────┤
│                     Domain                        │
│              (Entities │ Value Objects)           │
└─────────────────────────────────────────────────┘
```

## Domain Layer

Contains core business entities with no external dependencies.

```csharp
// Domain/Game.cs
public class Game
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int GenreId { get; set; }
    public Genre Genre { get; set; } = null!;
    public decimal Price { get; set; }
    public DateOnly ReleaseDate { get; set; }
}
```

## Application Layer

Contains business logic with **MediatR** for CQRS separation.

### Commands (Write Operations)

- `CreateGameCommand` - Add new game
- `UpdateGameCommand` - Modify existing game
- `DeleteGameCommand` - Remove game

### Queries (Read Operations)

- `GetAllGamesQuery` - Retrieve all games
- `GetGameByIdQuery` - Retrieve single game

### Handlers

Each command/query has a corresponding handler that implements business logic:

```csharp
public class GetAllGamesHandler : IRequestHandler<GetAllGamesQuery, IEnumerable<GameDto>>
{
    public async Task<IEnumerable<GameDto>> Handle(GetAllGamesQuery request, CancellationToken cancellationToken)
    {
        // Implementation
    }
}
```

### Ports (Interfaces)

Repository interfaces define data access contracts:

```csharp
// Application/Ports/IGameRepository.cs
public interface IGameRepository
{
    Task<IEnumerable<Game>> GetAllAsync(CancellationToken ct = default);
    Task<Game?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Game> CreateAsync(Game game, CancellationToken ct = default);
    Task UpdateAsync(Game game, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}
```

## Infrastructure Layer

Implements ports using Entity Framework Core.

### DbContext

```csharp
public class GameStoreContext : DbContext
{
    public DbSet<Game> Games => Set<Game>();
    public DbSet<Genre> Genres => Set<Genre>();
}
```

### Repositories

```csharp
public class GameRepository : IGameRepository
{
    private readonly GameStoreContext _context;
    
    public GameRepository(GameStoreContext context)
    {
        _context = context;
    }
}
```

## Endpoints Layer

Minimal API routes using `MapGroup`:

```csharp
public static class GameEndpoints
{
    public static void MapGameEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/games").WithTags("Games");
        
        group.MapGet("/", async (IMediator mediator) => 
            await mediator.Send(new GetAllGamesQuery()));
            
        group.MapPost("/", async (CreateGameDto dto, IMediator mediator) =>
            await mediator.Send(new CreateGameCommand(dto)));
    }
}
```

## Request Flow

```
HTTP Request
    ↓
Minimal API Endpoint
    ↓
MediatR Command/Query
    ↓
Handler
    ↓
Repository
    ↓
EF Core → SQLite
```

## Seed Data

The application seeds sample data on startup:

**Genres:** Action, Adventure, RPG, Strategy, Simulation

**Sample Games:**
- The Legend of Zelda: Breath of the Wild (Action)
- The Witcher 3: Wild Hunt (RPG)
- Civilization VI (Strategy)
- Microsoft Flight Simulator (Simulation)
- DOOM Eternal (Action)
