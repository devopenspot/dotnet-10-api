using GameStore.Api.Application.Queries;
using GameStore.Api.Domain;
using GameStore.Api.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Application.Handlers;

public class GetAllGamesQueryHandler(GameStoreContext context) : IRequestHandler<GetAllGamesQuery, List<Game>>
{
	public async Task<List<Game>> Handle(GetAllGamesQuery request, CancellationToken cancellationToken)
	{
		return await context.Set<Game>().AsNoTracking().ToListAsync(cancellationToken);
	}
}
