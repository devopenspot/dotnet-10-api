# Getting Started

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- IDE: VS Code, Rider, or Visual Studio 2022+

## Setup

### 1. Clone the Repository

```bash
git clone <repository-url>
cd dotnet-10-api
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Run the Application

```bash
cd GameStore.Api
dotnet run
```

The API starts at `http://localhost:5167` with automatic database migration and seed data.

### 4. Verify Installation

```bash
# Test the health endpoint
curl http://localhost:5167/games
```

Expected response:
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

## Development Tools

### HTTP Client

Use `Games.http` in the root directory to test endpoints directly from VS Code:

```bash
# Open Games.http and click "Send Request"
```

### Database Inspection

The SQLite database is at `GameStore.Api/GameStore.db`. Use any SQLite viewer or:

```bash
dotnet tool install --global dotnet-sqlite
sqlite3 GameStore.db ".tables"
```

## Troubleshooting

### Port Already in Use

```bash
# Find and kill the process
dotnet build && dotnet run --urls "http://localhost:5168"
```

### Database Reset

```bash
rm GameStore.Api/GameStore.db
dotnet run --project GameStore.Api
```

### Clean Rebuild

```bash
dotnet clean
dotnet restore
dotnet build
```
