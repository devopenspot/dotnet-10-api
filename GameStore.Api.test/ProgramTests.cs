using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GameStore.Api.Infrastructure;
using GameStore.Api.Application.Cache;
using System.Net;

namespace GameStore.Api.test;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		builder.ConfigureServices(services =>
		{
			var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<GameStoreContext>));
			if (descriptor is not null)
			{
				services.Remove(descriptor);
			}

			services.AddDbContext<GameStoreContext>(options =>
			{
				options.UseSqlite("Data Source=gamestore_test.db");
			});

			var cacheDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IGameQueryService));
			if (cacheDescriptor is not null)
			{
				services.Remove(cacheDescriptor);
			}
			services.AddScoped<IGameQueryService, InMemoryGameQueryService>();
		});
	}
}

public class ProgramTests : IClassFixture<CustomWebApplicationFactory>
{
	private readonly CustomWebApplicationFactory _factory;

	public ProgramTests(CustomWebApplicationFactory factory)
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
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}

	[Fact]
	public void Program_ShouldMigrateDatabase()
	{
		using var scope = _factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetService<GameStoreContext>();
		Assert.NotNull(context);
		Assert.True(context.Database.CanConnect());
	}
}
