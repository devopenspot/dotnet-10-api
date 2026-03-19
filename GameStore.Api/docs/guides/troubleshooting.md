# Troubleshooting Guide

Solutions to common issues encountered while developing with the GameStore API.

## Table of Contents

- [Database Issues](#database-issues)
- [Build & Runtime Issues](#build--runtime-issues)
- [Docker Issues](#docker-issues)
- [Redis Issues](#redis-issues)
- [Testing Issues](#testing-issues)

---

## Database Issues

### "Connection refused" / "Could not connect to server"

**Cause:** PostgreSQL container is not running.

**Solution:**

```bash
# Check container status
docker compose ps

# Start PostgreSQL
docker compose up -d postgres

# Verify it's running
docker compose logs postgres
```

### "Database does not exist"

**Cause:** Database hasn't been created.

**Solution:**

The API auto-creates the database on first run. If using SQLite:

```bash
# Delete existing database
rm GameStore.Api/gamestore.db

# Run API to recreate
dotnet run --project GameStore.Api
```

For PostgreSQL:

```bash
# Connect to PostgreSQL
docker compose exec postgres psql -U postgres -d postgres

# Create database
CREATE DATABASE gamestore;
\q
```

### "No migrations applied"

**Cause:** Migration files exist but haven't been applied.

**Solution:**

```bash
cd GameStore.Api

# Apply all migrations
dotnet ef database update

# Or run the API (auto-migrates on startup)
dotnet run
```

### "Relation does not exist"

**Cause:** Database schema doesn't match code.

**Solution:**

```bash
# Remove migrations and re-create
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

## Build & Runtime Issues

### "The SDK 'Microsoft.NET.Sdk' could not be found"

**Cause:** .NET 10 SDK not installed.

**Solution:**

```bash
# Check installed SDKs
dotnet --list-sdks

# Install .NET 10 SDK
# macOS
brew install dotnet@10

# Linux
curl -sSL https://dot.net/v1/dotnet-install.sh | bash -s -- --version 10.0.0

# Windows
winget install Microsoft.DotNet.SDK.10
```

### "Project file could not be found"

**Cause:** Not in the correct directory or solution not loaded.

**Solution:**

```bash
# Navigate to project
cd dotnet-10-api

# Restore and build
dotnet restore
dotnet build

# Or open solution file
dotnet sln GameStore.Api.slnx open
```

### "Could not find 'GameStore.Api'"

**Cause:** Wrong project name in command.

**Solution:**

```bash
# List available projects
dotnet sln GameStore.Api.slnx list

# Use correct project path
dotnet run --project GameStore.Api
dotnet test --project GameStore.Api.test
```

### Port already in use (8080)

**Cause:** Another process is using the port.

**Solution:**

```bash
# Find process using port 8080
# macOS/Linux
lsof -i :8080

# Windows
netstat -ano | findstr :8080

# Kill the process
kill -9 <PID>

# Or use a different port in launchSettings.json
```

---

## Docker Issues

### "Container 'postgres' is unhealthy"

**Cause:** PostgreSQL failed to start or became unresponsive.

**Solution:**

```bash
# Remove and recreate container
docker compose down -v
docker compose up -d postgres

# Check logs
docker compose logs postgres
```

### "Volume mount denied"

**Cause:** Docker doesn't have permission to access the directory.

**Solution:**

```bash
# Linux: Check directory permissions
chmod 755 .
ls -la

# macOS: Grant Docker Desktop file access
# Docker Desktop → Settings → Resources → File Sharing

# Or use named volumes (already configured)
docker compose down
docker compose up -d
```

### "Network connection refused to container"

**Cause:** Container networking issue.

**Solution:**

```bash
# Recreate network
docker compose down
docker network prune
docker compose up -d

# Verify network
docker network inspect dotnet-10-api_default
```

### Docker Desktop not running

**Cause:** Docker daemon not started.

**Solution:**

```bash
# Start Docker Desktop application

# Verify
docker info
```

---

## Redis Issues

### Redis Connection Refused

**Cause:** Redis container not running or not accessible.

**Solution:**

```bash
# Start Redis
docker compose up -d redis

# Verify
docker compose exec redis redis-cli ping
# Should return: PONG
```

### "It was not possible to connect to the Redis server"

**Cause:** Wrong Redis connection string.

**Solution:**

Check `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "RedisConnection": "localhost:6379"
  }
}
```

**Important:** When using Docker, use `localhost` from host, or `redis` from another container.

### Redis Performance Issues

**Cause:** Cache misses or slow Redis instance.

**Solution:**

```bash
# Monitor Redis
docker compose exec redis redis-cli info stats

# Flush cache (dev only)
docker compose exec redis redis-cli FLUSHALL
```

**Note:** Redis is optional. The API falls back to in-memory caching when Redis is unavailable.

---

## Testing Issues

### "Test run failed - No test is available"

**Cause:** Test project not referenced or build failed.

**Solution:**

```bash
cd GameStore.Api.test
dotnet restore
dotnet build
dotnet test
```

### Integration tests hang

**Cause:** Database connection not released.

**Solution:**

Ensure tests properly dispose resources:

```csharp
public class MyTests : IDisposable
{
    private readonly GameStoreContext _context;

    public MyTests()
    {
        _context = new GameStoreContext(options);
        _context.Database.OpenConnection();
    }

    public void Dispose()
    {
        _context.Database.CloseConnection();
        _context.Dispose();
    }
}
```

### "Cannot resolve 'Microsoft.AspNetCore.Mvc.Testing'"

**Cause:** Package not restored.

**Solution:**

```bash
dotnet restore GameStore.Api.test
```

### Test database locked (SQLite)

**Cause:** Multiple tests accessing same file.

**Solution:**

Use in-memory database:

```csharp
var options = new DbContextOptionsBuilder<GameStoreContext>()
    .UseSqlite("DataSource=:memory:")
    .Options;
```

---

## General Issues

### "DLL not found" / "Runtime error"

**Cause:** Build artifacts corrupted.

**Solution:**

```bash
# Clean and rebuild
dotnet clean
rm -rf bin obj
dotnet restore
dotnet build
```

### Slow startup time

**Cause:** Too many packages or slow EF migrations.

**Solution:**

```bash
# Use watch mode for faster iteration
dotnet watch --project GameStore.Api

# Skip migrations check in dev (optional)
# Modify DataExtensions.MigrateDatabase() to conditionally migrate
```

### Environment variables not loaded

**Cause:** Variables set after launch or wrong format.

**Solution:**

```bash
# Set and run in same command
DB_PASSWORD=secret dotnet run --project GameStore.Api

# Export first (bash/zsh)
export DB_PASSWORD=secret
dotnet run --project GameStore.Api
```

---

## Getting Help

If you encounter an issue not covered here:

1. **Check logs:** Look at console output for detailed error messages
2. **Search existing issues:** Check project issue tracker
3. **Ask in team channel:** Describe the issue + steps to reproduce

### Useful Commands

```bash
# Get diagnostic info
dotnet --version
dotnet --list-sdks
docker --version
docker compose version

# Clean everything
docker compose down -v --remove-orphans
rm -rf **/bin **/obj **/*.db
dotnet restore
```

---

## Next Steps

- [Getting Started](./getting-started.md) - Initial setup
- [Configuration](./configuration.md) - Environment setup
- [Architecture](../architecture/README.md) - Understanding the codebase
