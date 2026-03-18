using GameStore.Api.Application;
using GameStore.Api.Domain;
using GameStore.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

/// <summary>
/// Contains endpoint mappings for game-related operations.
/// </summary>
public static class GamesEndpoints
{
	const string GetGameEndpoint = "GetGame";

	/// <summary>
	/// Maps the game endpoints to the web application.
	/// </summary>
	/// <param name="app">The web application.</param>
	public static void MapGamesEndpoints(this WebApplication app)
	{
		var Group = app.MapGroup("/games");

		Group.MapGet("/", async (GameStoreContext dbContext) =>
		{
			return Results.Ok(await dbContext.Set<Game>().AsNoTracking().ToListAsync());
		});

		Group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
		{
			var game = await dbContext.Set<Game>().FindAsync(id);
			return game is not null ? Results.Ok(game) : Results.NotFound();
		});

		Group.MapPost("/", async (CreateGameDto game, GameStoreContext dbContext) =>
		{
			var newGame = new Game
			{
				Name = game.Name,
				GenreId = game.GenreId,
				Price = game.Price,
				ReleaseDate = game.ReleaseDate
			};
			dbContext.Add(newGame);
			await dbContext.SaveChangesAsync();
			return Results.Created($"/{newGame.Id}", newGame);
		});

		Group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
		{
			var game = await dbContext.Set<Game>().FindAsync(id);
			if (game is null)
			{
				return Results.NotFound();
			}

			game.Name = updatedGame.Name;
			game.GenreId = updatedGame.GenreId;
			game.Price = updatedGame.Price;
			game.ReleaseDate = updatedGame.ReleaseDate;

			await dbContext.SaveChangesAsync();
			return Results.Ok(game);
		});

		Group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
		{
			var game = await dbContext.Set<Game>().FindAsync(id);
			if (game is null)
			{
				return Results.NotFound();
			}

			dbContext.Remove(game);
			await dbContext.SaveChangesAsync();
			return Results.NoContent();
		});
	}
}