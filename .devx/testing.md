# Testing

## Overview

The project uses **xUnit** for testing with **Microsoft.AspNetCore.Mvc.Testing** for integration tests.

## Running Tests

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific project
dotnet test GameStore.Api.test
```

## Test Structure

```
GameStore.Api.test/
├── Unit/
│   ├── DtosTests.cs
│   ├── ModelsTests.cs
│   └── DataTests.cs
└── Integration/
    └── EndpointsIntegrationTests.cs
```

## Unit Tests

### Model Tests

```csharp
public class ModelsTests
{
    [Fact]
    public void Game_DefaultValues_AreCorrect()
    {
        var game = new Game();
        
        Assert.Equal(string.Empty, game.Name);
        Assert.Equal(0, game.Price);
    }
}
```

### DTO Tests

```csharp
public class DtosTests
{
    [Fact]
    public void CreateGameDto_Validation_FailsWhenNameEmpty()
    {
        var dto = new CreateGameDto("", 1, 59.99m, DateOnly.FromDateTime(DateTime.Now));
        var validationResults = ValidateModel(dto);
        
        Assert.Contains(validationResults, v => v.MemberNames.Contains("Name"));
    }
}
```

## Integration Tests

### Web Application Factory

```csharp
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext
            // Add test-specific configuration
        });
    }
}
```

### Endpoint Tests

```csharp
public class EndpointsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    
    public EndpointsIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }
    
    [Fact]
    public async Task GetGames_ReturnsSuccessStatusCode()
    {
        var response = await _client.GetAsync("/games");
        
        response.EnsureSuccessStatusCode();
    }
}
```

## Writing Tests

### Test Naming

```
MethodName_Scenario_ExpectedResult
```

Examples:
- `GetAllGames_ReturnsEmptyList_WhenNoGamesExist`
- `CreateGame_ReturnsCreatedGame_WhenValidInput`
- `UpdateGame_ReturnsNotFound_WhenGameDoesNotExist`

### Best Practices

1. **Arrange-Act-Assert** pattern
2. One assertion per test when practical
3. Use descriptive test names
4. Test both success and failure paths
5. Mock external dependencies in unit tests

```csharp
[Fact]
public async Task CreateGame_ReturnsCreatedGame_WhenValidInput()
{
    // Arrange
    var dto = new CreateGameDto("Test Game", 1, 49.99m, DateOnly.FromDateTime(DateTime.Today));
    
    // Act
    var response = await _client.PostAsJsonAsync("/games", dto);
    
    // Assert
    response.EnsureSuccessStatusCode();
    var game = await response.Content.ReadFromJsonAsync<GameDto>();
    Assert.Equal("Test Game", game!.Name);
}
```
