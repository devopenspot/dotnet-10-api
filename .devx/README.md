# GameStore API

A modern RESTful API for managing video games and genres, built with .NET 10 using Clean Architecture, CQRS, PostgreSQL, and Redis.

## Quick Start

### Docker (Full Stack)
```bash
docker-compose up -d
# API: http://localhost:8080
```

### Local (SQLite)
```bash
cd GameStore.Api
dotnet run
# API: http://localhost:5167
```

## Architecture

```
Commands (Write) → PostgreSQL → Redis (invalidate)
Queries (Read) → Redis Cache → PostgreSQL (on miss)
```

## Tech Stack

| Component | Technology |
|-----------|------------|
| Runtime | .NET 10 |
| API | ASP.NET Core Minimal APIs |
| Commands | PostgreSQL + EF Core |
| Queries | Redis Cache |
| CQRS | MediatR |
| Container | Docker Compose |

## Documentation

- [Getting Started](./getting-started.md) - Setup guide
- [Architecture](./architecture.md) - Design patterns
- [Docker](./docker.md) - Container orchestration
- [API Reference](./api-reference.md) - Endpoint docs
- [Contributing](./contributing.md) - Development workflow
- [Testing](./testing.md) - Testing guidelines
- [CQRS Commands/Queries](./cqrs-commands-queries.md) - MediatR patterns
