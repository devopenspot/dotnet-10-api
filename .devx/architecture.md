# Architecture

## Overview

GameStore.Api follows **Clean Architecture** principles with **CQRS** pattern, using PostgreSQL for commands and Redis for query caching.

## Layer Structure

```
┌─────────────────────────────────────────────────┐
│                   Endpoints                       │
│            (Minimal API Routes)                  │
├─────────────────────────────────────────────────┤
│                  Application                      │
│  Commands │ Queries │ Handlers │ DTOs │ Cache   │
├─────────────────────────────────────────────────┤
│                   Infrastructure                  │
│  EF Core (PostgreSQL) │ Repositories │ Redis     │
├─────────────────────────────────────────────────┤
│                     Domain                        │
│              (Entities │ Value Objects)           │
└─────────────────────────────────────────────────┘
```

## CQRS Pattern

**Commands** (Write) → PostgreSQL
**Queries** (Read) → Redis Cache

### Data Flow

```
┌─────────────────────────────────────────────────────────────────┐
│                        WRITE PATH                                │
│  HTTP → Endpoint → Command → Handler → Repository → PostgreSQL  │
│                                   ↓                              │
│                              Notification → Redis (invalidate)    │
└─────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────┐
│                        READ PATH                                 │
│  HTTP → Endpoint → Query → Handler → Redis Cache → (miss: DB)   │
└─────────────────────────────────────────────────────────────────┘
```

## Domain Layer

Core business entities with no external dependencies.

```csharp
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

### Commands (Write Operations)

- `CreateGameCommand` → PostgreSQL
- `UpdateGameCommand` → PostgreSQL
- `DeleteGameCommand` → PostgreSQL

### Queries (Read Operations)

- `GetAllGamesQuery` → Redis Cache
- `GetGameByIdQuery` → Redis Cache

### Event-Driven Synchronization

Commands dispatch notifications that update the Redis cache:

```csharp
// Command Handler
var created = await repository.CreateAsync(game, ct);
await publisher.Publish(new GameCreatedNotification(
    created.Id, created.Name, created.GenreId, created.Price, created.ReleaseDate), ct);

// Notification Handler (updates Redis)
public async Task Handle(GameCreatedNotification notification, CancellationToken ct)
{
    var dto = new GameDto(notification.Id, notification.Name, ...);
    await _queryService.SetGameAsync(dto, ct);
    await _queryService.InvalidateAllGamesAsync(ct);
}
```

### Cache Service

```csharp
public interface IGameQueryService
{
    Task<List<GameDto>?> GetAllGamesAsync(CancellationToken ct = default);
    Task<GameDto?> GetGameByIdAsync(int id, CancellationToken ct = default);
    Task SetAllGamesAsync(List<GameDto> games, CancellationToken ct = default);
    Task SetGameAsync(GameDto game, CancellationToken ct = default);
    Task InvalidateAllGamesAsync(CancellationToken ct = default);
    Task InvalidateGameAsync(int id, CancellationToken ct = default);
}
```

## Infrastructure Layer

### Database Providers

- **PostgreSQL** (production): `Npgsql.EntityFrameworkCore.PostgreSQL`
- **SQLite** (local dev): `Microsoft.EntityFrameworkCore.Sqlite`

```csharp
// Auto-detects based on connection string
if (connectionString.Contains("Host="))
    options.UseNpgsql(connectionString);
else
    options.UseSqlite(connectionString);
```

### Redis Cache

Uses `StackExchange.Redis` for distributed caching with 10-minute TTL.

## Request Flow

### Write (Command)
```
HTTP POST/PUT/DELETE → Endpoint → MediatR Command → Handler → PostgreSQL
                                                        ↓
                                            MediatR Notification
                                                        ↓
                                            Redis (invalidate cache)
```

### Read (Query)
```
HTTP GET → Endpoint → MediatR Query → Handler → Redis Cache
                                               ↓ (cache miss)
                                           PostgreSQL → Redis
```

## Seed Data

Sample data is seeded on application startup:

**Genres:** Action, Adventure, RPG, Strategy, Simulation

**Sample Games:**
- The Legend of Zelda: Breath of the Wild
- The Witcher 3: Wild Hunt
- Civilization VI
- Microsoft Flight Simulator
- DOOM Eternal
