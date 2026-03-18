using GameStore.Api.Domain;
using GameStore.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

public static class DataExtensions
{
	/// <summary>
	/// Migrates the database to the latest version.
	/// </summary>
	/// <param name="app">The web application.</param>
	public static void MigrateDatabase(this WebApplication app)
	{
		using var scope = app.Services.CreateScope();
		var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();
		dbContext.Database.Migrate();
	}

	/// <summary>
	/// Configures the GameStore database services.
	/// </summary>
	/// <param name="builder">The web application builder.</param>
	public static void GameStoreDatabase(this WebApplicationBuilder builder)
	{
		var connectionString = builder.Configuration.GetConnectionString("GameStoreConnection");
		// builder.Services.AddScoped<GameStoreContext>();
		// DbContext has a Scoped service lifetime because:
		// 1. It ensures that a new instance of DbContext is created per request
		// 2. DB connections are a limited and expensive resource
		// 3. DbContext is not thread-safe. Scoped avoids concurrency issues
		// 4. Makes it easier to manage transactions and ensure data consistency
		// 5. Reusing a DbContext instance can lead to increased memory usage

		builder.Services.AddSqlite<GameStoreContext>(
		connectionString, 
			optionsAction: options => options.UseSeeding((context, _) =>
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
			}
		));
	}
}