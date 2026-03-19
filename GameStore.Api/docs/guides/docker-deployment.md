# Docker Deployment Guide

Complete guide to running and deploying the GameStore API using Docker.

## Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (macOS/Windows)
- Docker Engine + Docker Compose (Linux)
- 2GB RAM minimum for containers

## Development Setup

### Start All Services

```bash
# Start PostgreSQL, Redis, and API
docker compose up -d

# Start only infrastructure (API manually)
docker compose up -d postgres redis
```

### Run API in Container

```bash
# Build and run API container
docker compose up -d --build api

# View logs
docker compose logs -f api

# Stop
docker compose down
```

### Access Services

| Service | Host | Container Port |
|---------|------|----------------|
| API | http://localhost:8080 | 8080 |
| PostgreSQL | localhost:5432 | 5432 |
| Redis | localhost:6379 | 6379 |

### Test Endpoints

```bash
# Health check
curl http://localhost:8080/games

# Create game
curl -X POST http://localhost:8080/games \
  -H "Content-Type: application/json" \
  -d '{"name":"Test Game","genreId":1,"price":29.99,"releaseDate":"2024-01-01"}'
```

## Docker Compose Files

### docker-compose.yml (Base)

```yaml
services:
  postgres:
    image: postgres:16-alpine
    environment:
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
      POSTGRES_DB: gamestore
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 5s
      timeout: 5s
      retries: 5

  redis:
    image: redis:7-alpine
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 5s
      timeout: 5s
      retries: 5

  api:
    build:
      context: .
      dockerfile: Dockerfile
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - DOTNET_RUNNING_IN_CONTAINER=true
      - ConnectionStrings__GameStoreConnection=Host=postgres;Port=5432;Database=gamestore;Username=postgres;Password=postgres
      - ConnectionStrings__RedisConnection=redis:6379
    depends_on:
      postgres:
        condition: service_healthy
      redis:
        condition: service_healthy

volumes:
  postgres_data:
  redis_data:
```

### docker-compose.override.yml (Development)

```yaml
services:
  api:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
    volumes:
      - ./GameStore.Api:/app
    build:
      target: development
```

## Dockerfile

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["GameStore.Api/GameStore.Api.csproj", "GameStore.Api/"]
RUN dotnet restore "GameStore.Api/GameStore.Api.csproj"
COPY . .
WORKDIR "/src/GameStore.Api"
RUN dotnet build "GameStore.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GameStore.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS development
RUN dotnet install tool:
COPY --from=build /app/build /app
WORKDIR /app
CMD ["dotnet", "GameStore.Api.dll"]

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GameStore.Api.dll"]
```

## Production Deployment

### Build Production Image

```bash
# Build without cache
docker build -t gamestore-api:latest .

# Build with tags
docker build \
  -t gamestore-api:latest \
  -t gamestore-api:1.0.0 \
  .
```

### Run Production Container

```bash
# Basic run
docker run -d \
  --name gamestore-api \
  -p 8080:8080 \
  -e ConnectionStrings__GameStoreConnection="Host=db.example.com;..." \
  -e ConnectionStrings__RedisConnection="redis.example.com:6379" \
  gamestore-api:latest
```

### Docker Compose Production

```yaml
# docker-compose.prod.yml
services:
  api:
    image: gamestore-api:latest
    restart: unless-stopped
    ports:
      - "80:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__GameStoreConnection=${DB_CONNECTION}
      - ConnectionStrings__RedisConnection=${REDIS_CONNECTION}
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/games"]
      interval: 30s
      timeout: 10s
      retries: 3
    deploy:
      resources:
        limits:
          memory: 512M
```

```bash
# Deploy
docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

## Kubernetes Deployment

### Deployment Manifest

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: gamestore-api
spec:
  replicas: 3
  selector:
    matchLabels:
      app: gamestore-api
  template:
    metadata:
      labels:
        app: gamestore-api
    spec:
      containers:
      - name: api
        image: gamestore-api:latest
        ports:
        - containerPort: 8080
        env:
        - name: ConnectionStrings__GameStoreConnection
          valueFrom:
            secretKeyRef:
              name: gamestore-secrets
              key: db-connection
        - name: ConnectionStrings__RedisConnection
          valueFrom:
            configMapKeyRef:
              name: gamestore-config
              key: redis-connection
        resources:
          limits:
            memory: "512Mi"
            cpu: "500m"
```

```bash
# Deploy
kubectl apply -f deployment.yaml
kubectl apply -f service.yaml
```

## Container Health Checks

### API Health Check

The API auto-reports health via `/games` endpoint. Docker health check:

```yaml
healthcheck:
  test: ["CMD", "curl", "-f", "http://localhost:8080/games"]
  interval: 30s
  timeout: 10s
  retries: 3
  start_period: 40s
```

### Verify Health

```bash
# Check container health
docker inspect --format='{{.State.Health.Status}}' gamestore-api_api_1

# View health logs
docker inspect gamestore-api_api_1 | grep -A 20 "Health"
```

## Logging

### View Container Logs

```bash
# All logs
docker compose logs api

# Follow logs
docker compose logs -f api

# Last 100 lines
docker compose logs --tail 100 api

# Timestamps
docker compose logs -t api
```

### Application Logs

Logs are written to stdout in Docker. Use log aggregators:

```bash
# JSON logs (Production)
docker run -d \
  --name gamestore-api \
  -e Logging__LogLevel__Default=Information \
  gamestore-api:latest

# Debug logs
docker run -d \
  --name gamestore-api \
  -e Logging__LogLevel__Default=Debug \
  gamestore-api:latest
```

## Resource Management

### Memory Limits

```yaml
services:
  api:
    deploy:
      resources:
        limits:
          memory: 512M
        reservations:
          memory: 256M
```

### CPU Limits

```yaml
services:
  api:
    deploy:
      resources:
        limits:
          cpu: 0.5
        reservations:
          cpu: 0.25
```

## Networking

### External Access

```yaml
services:
  api:
    ports:
      - "8080:8080"  # Host:Container
```

### Internal Network

Containers communicate via service names:

```yaml
# From API to PostgreSQL
Host=postgres
Port=5432

# From API to Redis
redis:6379
```

## Troubleshooting

### Container Won't Start

```bash
# View logs
docker compose logs api

# Check configuration
docker compose config

# Rebuild
docker compose build --no-cache api
```

### Database Connection Failed

```bash
# Verify PostgreSQL is healthy
docker compose ps postgres

# Test connection
docker compose exec postgres psql -U postgres -d gamestore

# Check logs
docker compose logs postgres
```

### Out of Memory

```bash
# Check container stats
docker stats

# Increase Docker Desktop memory
# Docker Desktop → Settings → Resources → Memory
```

## Cleanup

```bash
# Stop services
docker compose down

# Stop and remove volumes (loses data!)
docker compose down -v

# Remove images
docker rmi gamestore-api:latest

# Remove all stopped containers and unused images
docker system prune -a
```

## Security Best Practices

1. **Don't embed secrets** in images
2. **Use specific image tags** (not `latest`)
3. **Run as non-root** user
4. **Scan for vulnerabilities**: `docker scan gamestore-api`
5. **Use secrets** for sensitive data in production

## Next Steps

- [Configuration](./configuration.md) - Environment configuration
- [Troubleshooting](./troubleshooting.md) - Common issues
- [Architecture](../architecture/README.md) - Understanding the codebase
