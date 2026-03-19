using GameStore.Api.Application.Commands;
using GameStore.Api.Application.Ports;
using MediatR;

namespace GameStore.Api.Application.Handlers;

public class DeleteGameCommandHandler(IGameRepository repository) : IRequestHandler<DeleteGameCommand, bool>
{
	public async Task<bool> Handle(DeleteGameCommand request, CancellationToken cancellationToken)
	{
		return await repository.DeleteAsync(request.Id, cancellationToken);
	}
}
