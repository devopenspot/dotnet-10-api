using GameStore.Api.Application;
using GameStore.Api.Application.Commands;
using GameStore.Api.Application.Queries;
using GameStore.Api.Domain;
using MediatR;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
	public static void MapGamesEndpoints(this WebApplication app)
	{
		var group = app.MapGroup("/games");

		group.MapGet("/", async (IMediator mediator) =>
		{
			return Results.Ok(await mediator.Send(new GetAllGamesQuery()));
		});

		group.MapGet("/{id}", async (int id, IMediator mediator) =>
		{
			var game = await mediator.Send(new GetGameByIdQuery(id));
			return game is not null ? Results.Ok(game) : Results.NotFound();
		});

		group.MapPost("/", async (CreateGameDto dto, IMediator mediator) =>
		{
			var command = new CreateGameCommand(dto.Name, dto.GenreId, dto.Price, dto.ReleaseDate);
			var game = await mediator.Send(command);
			return Results.Created($"/games/{game.Id}", game);
		});

		group.MapPut("/{id}", async (int id, UpdateGameDto dto, IMediator mediator) =>
		{
			var command = new UpdateGameCommand(id, dto.Name, dto.GenreId, dto.Price, dto.ReleaseDate);
			var game = await mediator.Send(command);
			return game is not null ? Results.Ok(game) : Results.NotFound();
		});

		group.MapDelete("/{id}", async (int id, IMediator mediator) =>
		{
			var result = await mediator.Send(new DeleteGameCommand(id));
			return result ? Results.NoContent() : Results.NotFound();
		});
	}
}
