# GameStore.Api Documentation

Welcome to the GameStore API documentation. This is a .NET 10 REST API for managing a video game catalog, built with Clean Architecture principles.

## Quick Navigation

### [Getting Started](./guides/getting-started.md)
New to the project? Start here for setup instructions and your first API call.

### [Architecture](./architecture/README.md)
Understand the codebase structure, CQRS pattern, and data flow.

### [CQRS Deep Dive](./architecture/CQRS-pattern.md)
Detailed explanation of command/query separation and MediatR usage.

### [API Reference](./api/README.md)
Complete endpoint documentation with request/response examples.

### [Configuration](./guides/configuration.md)
Environment-specific settings and secrets management.

### [Docker Deployment](./guides/docker-deployment.md)
Container deployment and orchestration.

### [Development Guides](./guides/development.md)
Common development tasks and patterns.

### [Troubleshooting](./guides/troubleshooting.md)
Solutions to common issues.

### [Testing](../GameStore.Api.test/docs/README.md)
Testing patterns and practices for this project.

## Tech Stack

| Technology | Purpose |
|------------|---------|
| .NET 10 | Runtime framework |
| MediatR | CQRS pattern implementation |
| Entity Framework Core | Database ORM |
| PostgreSQL | Production database |
| SQLite | Development database |
| Redis | Query caching layer |
| xUnit | Testing framework |

## Project Structure

```
GameStore.Api/
├── Application/           # Business logic layer
│   ├── Commands/         # Write operations (CQRS)
│   ├── Queries/          # Read operations (CQRS)
│   ├── Handlers/        # MediatR request handlers
│   ├── Ports/           # Interface definitions
│   └── Cache/           # Caching abstractions
├── Domain/               # Domain entities
├── Endpoints/            # API route definitions
├── Infrastructure/       # External integrations
│   ├── Adapters/        # Repository implementations
│   └── Migrations/      # EF Core migrations
└── docs/                 # Documentation
```

## Key Features

- **CQRS Pattern**: Commands and Queries are separated for better maintainability
- **Auto-seeding**: Sample data (5 games, 5 genres) is seeded on first run
- **Dual Database**: SQLite for development, PostgreSQL for production
- **Redis Caching**: Query results are cached when Redis is available
- **Auto-migration**: Database migrations run automatically on startup

## Need Help?

- Check the [Getting Started guide](./guides/getting-started.md) for setup instructions
- Review [API examples](./api/README.md) for endpoint usage
- See [Architecture](./architecture/README.md) for code organization details
- See [Troubleshooting](./guides/troubleshooting.md) for common issues
