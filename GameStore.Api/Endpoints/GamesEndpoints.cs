using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
	const string GetGameEndpoint = "GetGame";
	private static readonly List<GameDto> games = new List<GameDto>
	{
		new (1, "The Legend of Zelda: Breath of the Wild", "Action-Adventure", 59.99m, new (2017, 3, 3)),
		new (2, "Super Mario Odyssey", "Platformer", 49.99m, new (2017, 10, 27)),
		new (3, "Red Dead Redemption 2", "Action-Adventure", 69.99m, new (2018, 10, 26))
	};
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