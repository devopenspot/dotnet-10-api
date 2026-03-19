using GameStore.Api.Application;
using GameStore.Api.Application.Commands;
using GameStore.Api.Application.Notifications;
using GameStore.Api.Application.Ports;
using GameStore.Api.Domain;
using MediatR;

namespace GameStore.Api.Application.Handlers;

public class UpdateGameCommandHandler(IGameRepository repository, IPublisher publisher) : IRequestHandler<UpdateGameCommand, GameDto?>
{
	public async Task<GameDto?> Handle(UpdateGameCommand request, CancellationToken cancellationToken)
	{
		var game = new Game
		{
			Id = request.Id,
			Name = request.Name,
			GenreId = request.GenreId,
			Price = request.Price,
			ReleaseDate = request.ReleaseDate
		};

		var updated = await repository.UpdateAsync(game, cancellationToken);
		if (updated is null)
		{
			return null;
		}

		var genre = await repository.GetGenreByIdAsync(updated.GenreId, cancellationToken);
		await publisher.Publish(new GameUpdatedNotification(
			updated.Id, updated.Name, updated.GenreId, updated.Price, updated.ReleaseDate), cancellationToken);
		return new GameDto(updated.Id, updated.Name, genre?.Name ?? "", updated.Price, updated.ReleaseDate);
	}
}
