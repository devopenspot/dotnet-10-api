using GameStore.Api.Application.Commands;
using GameStore.Api.Application.Notifications;
using GameStore.Api.Application.Ports;
using MediatR;

namespace GameStore.Api.Application.Handlers;

public class DeleteGameCommandHandler(IGameRepository repository, IPublisher publisher) : IRequestHandler<DeleteGameCommand, bool>
{
	public async Task<bool> Handle(DeleteGameCommand request, CancellationToken cancellationToken)
	{
		var deleted = await repository.DeleteAsync(request.Id, cancellationToken);
		if (deleted)
		{
			await publisher.Publish(new GameDeletedNotification(request.Id), cancellationToken);
		}
		return deleted;
	}
}
