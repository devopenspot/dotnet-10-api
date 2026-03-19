using GameStore.Api.Application.Cache;
using GameStore.Api.Application.Notifications;
using GameStore.Api.Application.Ports;
using GameStore.Api.Domain;
using MediatR;

namespace GameStore.Api.Application.Events;

public class GameNotificationHandler :
	INotificationHandler<GameCreatedNotification>,
	INotificationHandler<GameUpdatedNotification>,
	INotificationHandler<GameDeletedNotification>
{
	private readonly IGameQueryService _queryService;
	private readonly IGameRepository _repository;

	public GameNotificationHandler(IGameQueryService queryService, IGameRepository repository)
	{
		_queryService = queryService;
		_repository = repository;
	}

	public async Task Handle(GameCreatedNotification notification, CancellationToken ct)
	{
		var genre = await _repository.GetGenreByIdAsync(notification.GenreId, ct);
		var dto = new GameDto(notification.Id, notification.Name, genre?.Name ?? "", notification.Price, notification.ReleaseDate);
		await _queryService.SetGameAsync(dto, ct);
		await _queryService.InvalidateAllGamesAsync(ct);
	}

	public async Task Handle(GameUpdatedNotification notification, CancellationToken ct)
	{
		var genre = await _repository.GetGenreByIdAsync(notification.GenreId, ct);
		var dto = new GameDto(notification.Id, notification.Name, genre?.Name ?? "", notification.Price, notification.ReleaseDate);
		await _queryService.SetGameAsync(dto, ct);
		await _queryService.InvalidateAllGamesAsync(ct);
	}

	public async Task Handle(GameDeletedNotification notification, CancellationToken ct)
	{
		await _queryService.InvalidateGameAsync(notification.Id, ct);
		await _queryService.InvalidateAllGamesAsync(ct);
	}
}
