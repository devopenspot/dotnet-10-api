using GameStore.Api.Domain;
using GameStore.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

public static class DataExtensions
{
	public static void MigrateDatabase(this WebApplication app)
	{
		using var scope = app.Services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();
		dbContext.Database.Migrate();
	}

	public static void GameStoreDatabase(this WebApplicationBuilder builder)
	{
		var connectionString = builder.Configuration.GetConnectionString("GameStoreConnection")!;
		var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

		builder.Services.AddDbContext<GameStoreContext>(options =>
		{
			if (connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase))
			{
				options.UseNpgsql(connectionString, npgsql =>
				{
					npgsql.EnableRetryOnFailure(3);
				});
			}
			else
			{
			options.UseSqlite(connectionString);
			}

			options.UseSeeding((context, _) =>
			{
				if (!context.Set<Genre>().Any())
				{
					context.Set<Genre>().AddRange(
						new Genre { Name = "Action" },
						new Genre { Name = "Adventure" },
						new Genre { Name = "RPG" },
						new Genre { Name = "Strategy" },
						new Genre { Name = "Simulation" }
					);
					context.SaveChanges();
				}

				if (!context.Set<Game>().Any())
				{
					context.Set<Game>().AddRange(
						new Game { Name = "The Legend of Zelda: Breath of the Wild", GenreId = 2, ReleaseDate = new (2017, 3, 3) },
						new Game { Name = "The Witcher 3: Wild Hunt", GenreId = 3, ReleaseDate = new (2015, 5, 19) },
						new Game { Name = "Civilization VI", GenreId = 4, ReleaseDate = new (2016, 10, 25) },
						new Game { Name = "Microsoft Flight Simulator", GenreId = 5, ReleaseDate = new (2020, 8, 4) },
						new Game { Name = "DOOM Eternal", GenreId = 1, ReleaseDate = new (2020, 3, 10) }
					);
					context.SaveChanges();
				}
			});
		});
	}
}
