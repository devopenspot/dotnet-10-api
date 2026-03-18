using GameStore.Api.Application.Queries;
using GameStore.Api.Domain;
using GameStore.Api.Infrastructure;
using MediatR;

namespace GameStore.Api.Application.Handlers;

public class GetGameByIdQueryHandler(GameStoreContext context) : IRequestHandler<GetGameByIdQuery, Game?>
{
	public async Task<Game?> Handle(GetGameByIdQuery request, CancellationToken cancellationToken)
	{
		return await context.Set<Game>().FindAsync([request.Id], cancellationToken);
	}
}
