using GameStore.Api.Application.Ports;
using GameStore.Api.Application.Queries;
using GameStore.Api.Domain;
using MediatR;

namespace GameStore.Api.Application.Handlers;

public class GetGameByIdQueryHandler(IGameRepository repository) : IRequestHandler<GetGameByIdQuery, Game?>
{
	public async Task<Game?> Handle(GetGameByIdQuery request, CancellationToken cancellationToken)
	{
		return await repository.GetByIdAsync(request.Id, cancellationToken);
	}
}
