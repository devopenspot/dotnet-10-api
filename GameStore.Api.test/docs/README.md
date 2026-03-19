# GameStore.Api.test Documentation

This project contains unit and integration tests for the GameStore API.

## Quick Navigation

- [Running Tests](#running-tests)
- [Test Structure](#test-structure)
- [Test Patterns](#test-patterns)
- [Writing Tests](#writing-tests)

## Tech Stack

| Technology | Purpose |
|------------|---------|
| xUnit | Test framework |
| Microsoft.NET.Test.Sdk | Test runner |
| Microsoft.AspNetCore.Mvc.Testing | Integration testing |
| Microsoft.EntityFrameworkCore.Sqlite | In-memory database for tests |

## Running Tests

### Run All Tests

```bash
cd GameStore.Api.test
dotnet test
```

### Run with Coverage

```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Run Specific Test Class

```bash
dotnet test --filter "ClassName=EndpointsIntegrationTests"
```

### Run Tests by Category

```bash
dotnet test --filter "Category=Integration"
```

### Verbose Output

```bash
dotnet test -v n
```

## Test Structure

```
GameStore.Api.test/
├── EndpointsIntegrationTests.cs   # API endpoint tests
├── DtosTests.cs                   # DTO validation tests
├── DataTests.cs                   # Database tests
├── ModelsTests.cs                 # Domain model tests
├── ObjectMothers/                # Test data builders
│   ├── GameObjectMother.cs
│   ├── GameDtoObjectMother.cs
│   ├── GenreObjectMother.cs
│   └── ...
├── InMemoryGameQueryService.cs   # Mock for caching layer
└── docs/
    └── README.md
```

## Test Categories

### Unit Tests

**ModelsTests.cs** - Domain model behavior:
```csharp
[Fact]
public void Game_Name_IsRequired()
{
    var game = new Game { Name = "Valid Name" };
    Assert.NotNull(game.Name);
}
```

**DtosTests.cs** - DTO structure validation:
```csharp
[Fact]
public void GameDto_HasRequiredProperties()
{
    var dto = new GameDto(1, "Test", "Action", 59.99m, DateOnly.FromDateTime(DateTime.Now));
    Assert.Equal(1, dto.Id);
    Assert.Equal("Test", dto.Name);
}
```

### Integration Tests

**EndpointsIntegrationTests.cs** - Full HTTP request/response:
```csharp
[Fact]
public async Task GetAllGames_ReturnsOkWithGames()
{
    var client = await CreateClient();
    
    var response = await client.GetAsync("/games");
    
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    var games = await response.Content.ReadFromJsonAsync<List<GameDto>>();
    Assert.NotEmpty(games);
}
```

**DataTests.cs** - Database operations:
```csharp
[Fact]
public async Task GameRepository_CreateAsync_ReturnsGameWithId()
{
    await using var context = CreateDbContext();
    var repository = new GameRepository(context);
    var game = new Game { Name = "Test", GenreId = 1, Price = 9.99m, ReleaseDate = DateOnly.FromDateTime(DateTime.Now) };
    
    var result = await repository.CreateAsync(game);
    
    Assert.True(result.Id > 0);
}
```

## Test Patterns

### Object Mother Pattern

Object Mothers provide reusable test data builders:

```csharp
// Creating a valid game for tests
var game = GameObjectMother.CreateValid();

// Creating a game with custom values
var game = GameObjectMother.Create(name: "Custom Game", price: 29.99m);

// Creating multiple games
var games = GameObjectMother.CreateList(5);
```

### Available Object Mothers

| Class | Purpose |
|-------|---------|
| `GameObjectMother` | Create Game entities |
| `GenreObjectMother` | Create Genre entities |
| `GameDtoObjectMother` | Create GameDto objects |
| `GenreDtoObjectMother` | Create GenreDto objects |
| `CommandObjectMother` | Create command objects |

### Example: Using Object Mothers

```csharp
[Fact]
public async Task CreateGame_ReturnsCreatedGame()
{
    // Arrange
    var createDto = GameDtoObjectMother.CreateCreateDto();
    var client = await CreateClient();
    
    // Act
    var response = await client.PostAsJsonAsync("/games", createDto);
    
    // Assert
    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    var created = await response.Content.ReadFromJsonAsync<GameDto>();
    Assert.NotNull(created);
    Assert.Equal(createDto.Name, created.Name);
}
```

## Writing Tests

### 1. Add Test to Existing File

For endpoint tests, add to `EndpointsIntegrationTests.cs`:

```csharp
[Fact]
public async Task DeleteGame_ReturnsNoContent_WhenGameExists()
{
    // Arrange
    var client = await CreateClient();
    
    // Act
    var response = await client.DeleteAsync("/games/1");
    
    // Assert
    Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
}
```

### 2. Create New Test Class

```csharp
namespace GameStore.Api.test;

public class NewFeatureTests
{
    [Fact]
    public void NewFeature_DoesExpectedBehavior()
    {
        // Arrange
        var input = "test";
        
        // Act
        var result = SomeMethod(input);
        
        // Assert
        Assert.Equal("expected", result);
    }
}
```

### 3. Test Database Setup

Use the shared helper methods:

```csharp
public class DataTests
{
    private GameStoreContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<GameStoreContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;
        
        var context = new GameStoreContext(options);
        context.Database.OpenConnection();
        context.Database.EnsureCreated();
        
        return context;
    }
}
```

### 4. Test HTTP Client Setup

For integration tests with HTTP:

```csharp
private async Task<HttpClient> CreateClient()
{
    var factory = new WebApplicationFactory<Program>();
    return factory.CreateClient();
}
```

## Mocking

### Mock Repository

For unit tests without database:

```csharp
var mockRepository = new Mock<IGameRepository>();
mockRepository
    .Setup(r => r.GetByIdAsync(It.IsAny<int>(), default))
    .ReturnsAsync(new Game { Id = 1, Name = "Test" });
```

### In-Memory Query Service

Use `InMemoryGameQueryService` for cached query tests:

```csharp
private IGameQueryService CreateQueryService()
{
    return new InMemoryGameQueryService();
}
```

## Best Practices

1. **AAA Pattern**: Arrange → Act → Assert
2. **Descriptive Names**: `DeleteGame_Returns404_WhenGameNotFound`
3. **One Assertion Focus**: Each test should verify one behavior
4. **Test Edge Cases**: Null, empty, boundary values
5. **Use Object Mothers**: Avoid magic values, use builders

## Coverage Goals

| Component | Target |
|-----------|--------|
| Endpoints | 90%+ |
| Handlers | 85%+ |
| Repository | 80%+ |
| Domain | 70%+ |

## CI/CD

Tests run automatically on:
- Pull request creation
- Push to main branch
- Manual trigger

Check repository CI configuration for details.
