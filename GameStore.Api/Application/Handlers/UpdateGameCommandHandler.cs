using GameStore.Api.Application.Commands;
using GameStore.Api.Domain;
using GameStore.Api.Infrastructure;
using MediatR;

namespace GameStore.Api.Application.Handlers;

public class UpdateGameCommandHandler(GameStoreContext context) : IRequestHandler<UpdateGameCommand, Game?>
{
	public async Task<Game?> Handle(UpdateGameCommand request, CancellationToken cancellationToken)
	{
		var game = await context.Set<Game>().FindAsync([request.Id], cancellationToken);
		if (game is null)
		{
			return null;
		}

		game.Name = request.Name;
		game.GenreId = request.GenreId;
		game.Price = request.Price;
		game.ReleaseDate = request.ReleaseDate;

		await context.SaveChangesAsync(cancellationToken);
		return game;
	}
}
