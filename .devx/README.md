# GameStore API

A modern RESTful API for managing video games and genres, built with .NET 10 using Clean Architecture and CQRS patterns.

## Quick Start

```bash
# Clone and run
cd GameStore.Api
dotnet run

# API available at http://localhost:5167
```

## Project Structure

```
GameStore.Api/
├── Domain/           # Core business entities
├── Application/      # Commands, Queries, Handlers, DTOs
├── Infrastructure/    # Data access, repositories, EF Core
├── Endpoints/         # Minimal API route definitions
└── Program.cs         # Application entry point
```

## Key Technologies

| Technology | Purpose |
|------------|---------|
| .NET 10 | Runtime |
| ASP.NET Core Minimal APIs | HTTP endpoints |
| MediatR | CQRS pattern |
| Entity Framework Core | Database ORM |
| SQLite | Database |
| xUnit | Testing |

## Resources

- [Getting Started](./getting-started.md) - Complete setup guide
- [Architecture](./architecture.md) - Deep dive into design
- [API Reference](./api-reference.md) - Endpoint documentation
- [Contributing](./contributing.md) - Development workflow
- [Testing](./testing.md) - Testing guidelines
