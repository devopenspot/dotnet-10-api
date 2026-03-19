using GameStore.Api.Application.Ports;
using GameStore.Api.Application.Queries;
using GameStore.Api.Domain;
using MediatR;

namespace GameStore.Api.Application.Handlers;

public class GetAllGamesQueryHandler(IGameRepository repository) : IRequestHandler<GetAllGamesQuery, List<Game>>
{
	public async Task<List<Game>> Handle(GetAllGamesQuery request, CancellationToken cancellationToken)
	{
		return await repository.GetAllAsync(cancellationToken);
	}
}
