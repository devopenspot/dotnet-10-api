# Architecture Overview

The GameStore API follows **Clean Architecture** principles with a clear separation of concerns.

## Layer Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     Endpoints Layer                          │
│              (GamesEndpoints.cs - Routes)                   │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    Application Layer                        │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐ │
│  │  Commands    │  │   Queries   │  │   Notifications     │ │
│  └─────────────┘  └─────────────┘  └─────────────────────┘ │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐ │
│  │  Handlers   │  │    DTOs     │  │   Ports (Interfaces)│ │
│  └─────────────┘  └─────────────┘  └─────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                   Infrastructure Layer                      │
│  ┌─────────────────────┐  ┌─────────────────────────────┐  │
│  │   GameRepository    │  │     GameStoreContext        │  │
│  │   (Adapter Pattern) │  │     (EF Core DbContext)     │  │
│  └─────────────────────┘  └─────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                      Domain Layer                            │
│              ┌─────────────┐  ┌─────────────┐              │
│              │    Game      │  │    Genre     │              │
│              └─────────────┘  └─────────────┘              │
└─────────────────────────────────────────────────────────────┘
```

## CQRS Pattern

The application uses **Command Query Responsibility Segregation** via MediatR.

### Commands (Write Operations)

Commands are fire-and-forget operations that modify state:

| Command | Handler | Purpose |
|---------|---------|---------|
| `CreateGameCommand` | `CreateGameCommandHandler` | Add new game |
| `UpdateGameCommand` | `UpdateGameCommandHandler` | Modify existing game |
| `DeleteGameCommand` | `DeleteGameCommandHandler` | Remove game |

### Queries (Read Operations)

Queries retrieve data without side effects:

| Query | Handler | Purpose |
|-------|---------|---------|
| `GetAllGamesQuery` | `GetAllGamesQueryHandler` | List all games |
| `GetGameByIdQuery` | `GetGameByIdQueryHandler` | Get single game |

### Why CQRS?

- **Separation of Concerns**: Read and write logic are isolated
- **Testability**: Each handler can be tested independently
- **Flexibility**: Different data models for reads vs writes
- **Scalability**: Reads and writes can be optimized separately

## Port & Adapter Pattern

### Ports (Interfaces)

Defined in `Application/Ports/`:

```csharp
public interface IGameRepository
{
    Task<List<Game>> GetAllAsync(CancellationToken ct = default);
    Task<Game?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Game> CreateAsync(Game game, CancellationToken ct = default);
    Task<Game?> UpdateAsync(Game game, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
```

### Adapters (Implementations)

Located in `Infrastructure/Adapters/`:

```csharp
public class GameRepository : IGameRepository
{
    // PostgreSQL/SQLite implementation via EF Core
}
```

## Data Flow: Create Game

```
HTTP POST /games
       │
       ▼
GamesEndpoints.MapPost()
       │
       ▼
MediatR.Send(CreateGameCommand)
       │
       ▼
CreateGameCommandHandler
       │
       ├──► IGameRepository.CreateAsync()
       │         │
       │         ▼
       │    GameStoreContext (EF Core)
       │         │
       │         ▼
       │    PostgreSQL/SQLite
       │
       ▼
Returns Created GameDto
```

## Caching Strategy

### Query Caching with Redis

Read operations use a caching layer (`IGameQueryService`):

```csharp
public interface IGameQueryService
{
    Task<List<GameDto>> GetAllGamesAsync();
    Task<GameDto?> GetGameByIdAsync(int id);
}
```

**Cache Behavior:**
- Cache hit → Return cached data (fast)
- Cache miss → Query database, cache result, return data

### Cache Configuration

| Environment | Cache Provider |
|-------------|----------------|
| Development | `DistributedMemoryCache` (in-memory) |
| Production | `StackExchange.Redis` |

Cache is configured in `Program.cs`:

```csharp
if (!string.IsNullOrEmpty(redisConnection))
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConnection;
        options.InstanceName = "GameStore:";
    });
}
```

## Database Schema

### Entities

**Game**
| Field | Type | Constraints |
|-------|------|-------------|
| Id | int | PK, Auto-increment |
| Name | string | Required, Max 100 chars |
| GenreId | int | FK → Genre |
| Price | decimal | Required |
| ReleaseDate | DateOnly | Required |

**Genre**
| Field | Type | Constraints |
|-------|------|-------------|
| Id | int | PK, Auto-increment |
| Name | string | Required, Unique |

### Entity Relationship

```
Genre (1) ←──── (N) Game
```

## Seeded Data

The application seeds sample data on first run:

**Genres:**
- Action, Adventure, RPG, Strategy, Simulation

**Games:**
- The Legend of Zelda: Breath of the Wild (Adventure, 2017)
- The Witcher 3: Wild Hunt (RPG, 2015)
- Civilization VI (Strategy, 2016)
- Microsoft Flight Simulator (Simulation, 2020)
- DOOM Eternal (Action, 2020)

## Key Files Reference

| File | Purpose |
|------|---------|
| `Program.cs` | Application startup, DI configuration |
| `Endpoints/GamesEndpoints.cs` | Route definitions |
| `Application/Commands/*.cs` | Command definitions |
| `Application/Handlers/*.cs` | Business logic |
| `Application/Ports/IGameRepository.cs` | Repository interface |
| `Infrastructure/Adapters/GameRepository.cs` | Repository implementation |
| `Infrastructure/GameStoreContext.cs` | EF Core DbContext |

## Next Steps

- [API Reference](../api/README.md) - Complete endpoint documentation
- [CQRS Deep Dive](./CQRS-pattern.md) - Detailed MediatR usage
