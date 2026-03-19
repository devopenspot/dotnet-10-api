# Getting Started

This guide will help you set up and run the GameStore API on your local machine.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for PostgreSQL and Redis)
- Optional: [Postman](https://www.postman.com/) or [HTTP Client](https://httpie.io/) for API testing

## Setup Steps

### 1. Clone and Navigate

```bash
git clone <repository-url>
cd dotnet-10-api
```

### 2. Start Infrastructure Services

The API requires PostgreSQL and Redis. Start them using Docker Compose:

```bash
docker compose up -d postgres redis
```

Verify services are running:

```bash
docker compose ps
```

Expected output:
```
NAME                IMAGE               COMMAND                  SERVICE   CREATED   STATUS
dotnet-api-postgres postgres:16-alpine  "docker-entrypoint.s…"   postgres  ...       Up
dotnet-api-redis    redis:7-alpine      "docker-entrypoint.s…"   redis     ...       Up
```

### 3. Run the API

```bash
cd GameStore.Api
dotnet run
```

The API will:
- Start on `http://localhost:8080` (or port from `launchSettings.json`)
- Automatically run database migrations
- Seed sample data (5 genres, 5 games)

### 4. Verify the API

Health check:

```bash
curl http://localhost:8080/games
```

Expected response:
```json
[
  {
    "id": 1,
    "name": "The Legend of Zelda: Breath of the Wild",
    "genre": "Adventure",
    "price": 0.00,
    "releaseDate": "2017-03-03"
  },
  ...
]
```

## Development Commands

| Command | Description |
|---------|-------------|
| `dotnet run` | Run the API |
| `dotnet build` | Build the project |
| `dotnet watch` | Run with hot reload |
| `dotnet test` | Run tests |
| `dotnet ef migrations add <name>` | Create a new migration |
| `dotnet ef migrations remove` | Remove last migration |

## Configuration

Configuration is managed through `appsettings.json` and environment variables.

### Connection Strings

Default connections in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "GameStoreConnection": "Host=localhost;Port=5432;Database=gamestore;Username=postgres;Password=postgres",
    "RedisConnection": "localhost:6379"
  }
}
```

### Environment-Specific Settings

Create `appsettings.Development.json` for local overrides:

```json
{
  "ConnectionStrings": {
    "GameStoreConnection": "Data Source=gamestore.db"
  }
}
```

## Troubleshooting

### Database Connection Failed

Ensure PostgreSQL container is running:
```bash
docker compose ps postgres
```

If not running, restart:
```bash
docker compose up -d postgres
```

### Port Already in Use

If port 8080 is taken, modify `GameStore.Api/Properties/launchSettings.json`:

```json
{
  "applicationUrl": "http://localhost:8081"
}
```

### Redis Connection Issues

Redis is optional. If unavailable, the API falls back to in-memory caching. No action required.

## Next Steps

- Read the [Architecture Overview](./architecture/README.md) to understand the codebase
- Explore the [API Reference](./api/README.md) for endpoint details
- Learn about [Testing](../GameStore.Api.test/docs/README.md) to add features with confidence
