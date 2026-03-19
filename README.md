# GameStore API Documentation

Complete documentation for the GameStore API project.

## Project Overview

A .NET 10 REST API for managing a video game catalog, built with Clean Architecture, CQRS pattern, and modern best practices.

## Documentation Sections

### GameStore.Api
Main API project documentation.

| Document | Description |
|----------|-------------|
| [Getting Started](./GameStore.Api/docs/guides/getting-started.md) | Setup and first API call |
| [Architecture](./GameStore.Api/docs/architecture/README.md) | Architecture and design patterns |
| [CQRS Pattern](./GameStore.Api/docs/architecture/CQRS-pattern.md) | Deep dive into CQRS implementation |
| [API Reference](./GameStore.Api/docs/api/README.md) | Complete endpoint documentation |
| [Configuration](./GameStore.Api/docs/guides/configuration.md) | Environment configuration |
| [Docker Deployment](./GameStore.Api/docs/guides/docker-deployment.md) | Container deployment |
| [Development Guides](./GameStore.Api/docs/guides/development.md) | Common development tasks |
| [Troubleshooting](./GameStore.Api/docs/guides/troubleshooting.md) | Common issues and solutions |

### GameStore.Api.test
Testing project documentation.

| Document | Description |
|----------|-------------|
| [Testing Guide](./GameStore.Api.test/docs/README.md) | Testing patterns and practices |

## Quick Start

```bash
# Start infrastructure
docker compose up -d postgres redis

# Run the API
cd GameStore.Api
dotnet run

# Test endpoints
curl http://localhost:8080/games
```

## Architecture Highlights

- **CQRS Pattern**: Commands and Queries separated via MediatR
- **Clean Architecture**: Domain → Application → Infrastructure → Endpoints
- **Dual Database**: SQLite (dev) / PostgreSQL (prod)
- **Redis Caching**: Query result caching when available
- **Auto-migration**: Database schema updates on startup

## Tech Stack

```
.NET 10 │ MediatR │ Entity Framework Core │ PostgreSQL │ Redis │ xUnit
```

## Need Help?

1. New to the project? Start with [Getting Started](./GameStore.Api/docs/guides/getting-started.md)
2. Understanding code structure? See [Architecture](./GameStore.Api/docs/architecture/README.md)
3. Need to test endpoints? Check [API Reference](./GameStore.Api/docs/api/README.md)
4. Writing new features? Read [Development Guides](./GameStore.Api/docs/guides/development.md)
5. Deploying with Docker? See [Docker Deployment](./GameStore.Api/docs/guides/docker-deployment.md)
6. Facing issues? Check [Troubleshooting](./GameStore.Api/docs/guides/troubleshooting.md)
7. Writing tests? Review [Testing Guide](./GameStore.Api.test/docs/README.md)
