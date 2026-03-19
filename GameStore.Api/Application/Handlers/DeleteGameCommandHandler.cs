using GameStore.Api.Application.Commands;
using GameStore.Api.Application.Notifications;
using GameStore.Api.Application.Ports;
using MediatR;

namespace GameStore.Api.Application.Handlers;

public class DeleteGameCommandHandler(IGameRepository repository, IPublisher publisher) : IRequestHandler<DeleteGameCommand, bool>
{
	public async Task<bool> Handle(DeleteGameCommand request, CancellationToken cancellationToken)
	{
		var stopwatch = System.Diagnostics.Stopwatch.StartNew();
		try
		{
			var deleted = await repository.DeleteAsync(request.Id, cancellationToken);
			if (deleted)
			{
				await publisher.Publish(new GameDeletedNotification(request.Id), cancellationToken);
				AppMetrics.GamesDeleted.Add(1);
			}
			return deleted;
		}
		finally
		{
			stopwatch.Stop();
			AppMetrics.CommandHandlerDuration.Record(stopwatch.Elapsed.TotalMilliseconds);
		}
	}
}
