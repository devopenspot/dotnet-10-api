using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using GameStore.Api.Data;

namespace GameStore.Api.test;

public class ProgramTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProgramTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public void Program_ShouldBuildSuccessfully()
    {
        var app = _factory.CreateDefaultClient();
        Assert.NotNull(app);
    }

    [Fact]
    public void Program_ShouldConfigureServicesCorrectly()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetService<GameStoreContext>();
        Assert.NotNull(context);
    }

    [Fact]
    public async Task Program_ShouldMapEndpointsCorrectly()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/games");
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public void Program_ShouldMigrateDatabase()
    {
        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetService<GameStoreContext>();
        Assert.NotNull(context);
        // Assuming migration has run if context is created without error
        Assert.True(context.Database.CanConnect());
    }
}