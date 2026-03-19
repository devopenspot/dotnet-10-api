# Getting Started

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- IDE: VS Code, Rider, or Visual Studio 2022+

## Two Development Options

### Option A: Docker (Recommended for full stack)

```bash
# Start PostgreSQL + Redis + API
docker-compose up -d

# API available at http://localhost:8080
curl http://localhost:8080/games
```

### Option B: Local Development (SQLite + Memory Cache)

```bash
cd GameStore.Api
dotnet run

# API available at http://localhost:5167
```

## Quick Start (Local)

### 1. Clone and Setup

```bash
git clone <repository-url>
cd dotnet-10-api
dotnet restore
```

### 2. Run

```bash
cd GameStore.Api
dotnet run
```

### 3. Test

```bash
curl http://localhost:5167/games
```

Response:
```json
[
  {
    "id": 1,
    "name": "The Legend of Zelda: Breath of the Wild",
    "genre": "Action",
    "price": 59.99,
    "releaseDate": "2017-03-03"
  }
]
```

## Docker Setup

### Start Services

```bash
docker-compose up -d
```

### Verify Services

```bash
# Check API
curl http://localhost:8080/games

# Check PostgreSQL
docker-compose exec postgres pg_isready

# Check Redis
docker-compose exec redis redis-cli ping
```

### View Logs

```bash
docker-compose logs -f
```

### Stop Services

```bash
docker-compose down
```

## Development Tools

### HTTP Client

Use `Games.http` in the root directory:

```bash
# Open Games.http in VS Code and click "Send Request"
```

### Database Inspection

**SQLite (local):**
```bash
sqlite3 GameStore.Api/GameStore.db ".tables"
```

**PostgreSQL (Docker):**
```bash
docker-compose exec postgres psql -U postgres -d gamestore -c "SELECT * FROM \"Games\";"
```

## Troubleshooting

### Port Already in Use

```bash
# Find process using port
lsof -i :5167

# Use different port
dotnet run --urls "http://localhost:5168"
```

### Docker Connection Issues

```bash
# Restart Docker
docker-compose down
docker-compose up -d
```

### Clean Rebuild

```bash
# Local
dotnet clean && dotnet restore && dotnet build

# Docker
docker-compose down -v && docker-compose up -d --build
```
