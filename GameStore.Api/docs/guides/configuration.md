# Configuration Reference

Complete guide to configuring the GameStore API for different environments.

## Configuration Files

```
GameStore.Api/
├── appsettings.json              # Base configuration
├── appsettings.Development.json   # Development overrides
├── appsettings.Production.json    # Production overrides
└── .env.example                  # Environment template
```

## appsettings.json

Base configuration shared across all environments:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "GameStoreConnection": "Host=localhost;Port=5432;Database=gamestore;Username=postgres;Password=postgres",
    "RedisConnection": "localhost:6379"
  }
}
```

## Connection Strings

### PostgreSQL (Production)

```
Host=<server>;Port=5432;Database=<database>;Username=<user>;Password=<password>
```

**Parameters:**

| Parameter | Description | Default |
|-----------|-------------|---------|
| `Host` | Database server hostname | localhost |
| `Port` | Database port | 5432 |
| `Database` | Database name | gamestore |
| `Username` | Database user | postgres |
| `Password` | Database password | - |

### SQLite (Development)

```
DataSource=<path>.db
```

**Examples:**

```json
// Local file
"DataSource=gamestore.db"

// In-memory
"DataSource=:memory:"

// Relative path
"DataSource=../data/gamestore.db"
```

### Redis

```
<host>:<port>
```

**Examples:**

```json
// Local
"localhost:6379"

// With password
"localhost:6379,password=secret"

// Sentinel
"myserver:6379,serviceName=mymaster"
```

## Environment-Specific Configuration

### Development (appsettings.Development.json)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  },
  "ConnectionStrings": {
    "GameStoreConnection": "DataSource=gamestore.db"
  }
}
```

### Production (appsettings.Production.json)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Error"
    }
  },
  "ConnectionStrings": {
    "GameStoreConnection": "Host=prod-db.example.com;Port=5432;Database=gamestore;Username=prod_user;Password=${DB_PASSWORD}"
  }
}
```

## Environment Variables

Override configuration using environment variables:

### Syntax

```
ConnectionStrings__GameStoreConnection=<value>
```

### Common Variables

| Variable | Description | Example |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Runtime environment | `Development` |
| `ASPNETCORE_URLS` | Server URLs | `http://+:8080` |
| `ConnectionStrings__GameStoreConnection` | Database connection | `Host=localhost;...` |
| `ConnectionStrings__RedisConnection` | Redis connection | `localhost:6379` |
| `DOTNET_RUNNING_IN_CONTAINER` | Container detection | `true` |

### .env File

Copy `.env.example` to `.env`:

```bash
cp .env.example .env
```

Edit `.env`:

```env
ASPNETCORE_ENVIRONMENT=Development
GameStoreConnection=Host=localhost;Port=5432;Database=gamestore;Username=postgres;Password=postgres
RedisConnection=localhost:6379
```

> Note: The API doesn't load `.env` files automatically. Use `dotnet run` with `dotnetenv` or configure your IDE.

## Program.cs Configuration

### Service Registration

```csharp
// Program.cs - Key configuration points

// Database
builder.Services.AddDbContext<GameStoreContext>(options =>
{
    var connectionString = builder.Configuration
        .GetConnectionString("GameStoreConnection")!;
    
    // Auto-select provider based on connection string
    if (connectionString.Contains("Host="))
    {
        options.UseNpgsql(connectionString, npgsql =>
        {
            npgsql.EnableRetryOnFailure(3);  // Retry on transient failures
        });
    }
    else
    {
        options.UseSqlite(connectionString);
    }
});

// Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration
        .GetConnectionString("RedisConnection");
    options.InstanceName = "GameStore:";
});

// MediatR
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(CreateGameCommand).Assembly));
```

### Automatic Provider Detection

The API automatically selects the database provider:

| Connection String Contains | Provider |
|----------------------------|----------|
| `Host=` | PostgreSQL (via Npgsql) |
| `Data Source=` | SQLite |
| Other | SQLite (fallback) |

## Docker Configuration

### docker-compose.yml

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
```

### Container Environment Variables

```yaml
# docker-compose.override.yml
services:
  api:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - DOTNET_RUNNING_IN_CONTAINER=true
      - ConnectionStrings__GameStoreConnection=Host=postgres;Port=5432;Database=gamestore;Username=postgres;Password=postgres
      - ConnectionStrings__RedisConnection=redis:6379
```

## launchSettings.json

```json
{
  "profiles": {
    "GameStore.Api": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "http://localhost:8080",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

## Secrets Management

### Development

Store secrets in `secrets.json`:

```bash
cd GameStore.Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:GameStoreConnection" "Host=localhost;..."
```

### Production

Use environment variables or a secrets manager:

```bash
# Kubernetes Secret
kubectl create secret generic gamestore-secrets \
  --from-literal=DB_PASSWORD=secret123

# Azure Key Vault, AWS Secrets Manager, HashiCorp Vault, etc.
```

## Configuration Validation

The API validates configuration on startup. Missing required settings will cause startup failure with clear error messages.

## Quick Reference

### Common Configurations

**Local Development:**
```json
{
  "ConnectionStrings": {
    "GameStoreConnection": "DataSource=gamestore.db"
  }
}
```

**Docker Development:**
```json
{
  "ConnectionStrings": {
    "GameStoreConnection": "Host=localhost;Port=5432;Database=gamestore;Username=postgres;Password=postgres"
  }
}
```

**Production:**
```json
{
  "ConnectionStrings": {
    "GameStoreConnection": "${DB_CONNECTION_STRING}",
    "RedisConnection": "${REDIS_CONNECTION_STRING}"
  }
}
```

## Next Steps

- [Getting Started](./guides/getting-started.md) - Setup guide
- [Docker Deployment](./guides/docker-deployment.md) - Container deployment
- [Troubleshooting](./guides/troubleshooting.md) - Common issues
