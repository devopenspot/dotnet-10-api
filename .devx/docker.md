# Docker Development

Run the entire stack with PostgreSQL and Redis using Docker Compose.

## Quick Start

```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f api

# Stop services
docker-compose down
```

## Services

| Service | Port | Description |
|---------|------|-------------|
| `api` | 8080 | GameStore API |
| `postgres` | 5432 | PostgreSQL 16 (commands) |
| `redis` | 6379 | Redis 7 (query cache) |

## Local Development

### Without Docker

```bash
# Run API directly (uses SQLite)
dotnet run --project GameStore.Api
```

### With Docker

```bash
# Start infrastructure only
docker-compose up -d postgres redis

# Run API locally, connecting to Docker services
dotnet run --project GameStore.Api
```

## Environment Variables

Copy `.env.example` to `.env` and configure:

```bash
ConnectionStrings__GameStoreConnection=Host=localhost;Port=5432;Database=gamestore;Username=postgres;Password=postgres
ConnectionStrings__RedisConnection=localhost:6379
```

## Database Migrations

With PostgreSQL running:

```bash
dotnet ef migrations add <MigrationName> --project Infrastructure --startup-project GameStore.Api
dotnet ef database update --project Infrastructure --startup-project GameStore.Api
```

## Troubleshooting

### API can't connect to PostgreSQL

```bash
# Check if PostgreSQL is running
docker-compose ps postgres

# View PostgreSQL logs
docker-compose logs postgres
```

### Redis connection issues

```bash
# Check Redis health
docker-compose exec redis redis-cli ping
```

### Clean start

```bash
docker-compose down -v  # Remove volumes
docker-compose up -d    # Recreate everything
```
