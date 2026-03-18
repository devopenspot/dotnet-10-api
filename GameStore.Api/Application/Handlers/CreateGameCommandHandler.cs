using GameStore.Api.Application.Commands;
using GameStore.Api.Domain;
using GameStore.Api.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Application.Handlers;

public class CreateGameCommandHandler(GameStoreContext context) : IRequestHandler<CreateGameCommand, Game>
{
	public async Task<Game> Handle(CreateGameCommand request, CancellationToken cancellationToken)
	{
		var game = new Game
		{
			Name = request.Name,
			GenreId = request.GenreId,
			Price = request.Price,
			ReleaseDate = request.ReleaseDate
		};

		context.Set<Game>().Add(game);
		await context.SaveChangesAsync(cancellationToken);
		return game;
	}
}
