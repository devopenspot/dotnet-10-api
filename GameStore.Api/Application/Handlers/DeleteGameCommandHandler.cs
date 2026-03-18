using GameStore.Api.Application.Commands;
using GameStore.Api.Domain;
using GameStore.Api.Infrastructure;
using MediatR;

namespace GameStore.Api.Application.Handlers;

public class DeleteGameCommandHandler(GameStoreContext context) : IRequestHandler<DeleteGameCommand, bool>
{
	public async Task<bool> Handle(DeleteGameCommand request, CancellationToken cancellationToken)
	{
		var game = await context.Set<Game>().FindAsync([request.Id], cancellationToken);
		if (game is null)
		{
			return false;
		}

		context.Set<Game>().Remove(game);
		await context.SaveChangesAsync(cancellationToken);
		return true;
	}
}
