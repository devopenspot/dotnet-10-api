using GameStore.Api.Application;
using GameStore.Api.Application.Commands;
using GameStore.Api.Application.Notifications;
using GameStore.Api.Application.Ports;
using GameStore.Api.Domain;
using MediatR;

namespace GameStore.Api.Application.Handlers;

public class CreateGameCommandHandler(IGameRepository repository, IPublisher publisher) : IRequestHandler<CreateGameCommand, GameDto>
{
	public async Task<GameDto> Handle(CreateGameCommand request, CancellationToken cancellationToken)
	{
		var game = new Game
		{
			Name = request.Name,
			GenreId = request.GenreId,
			Price = request.Price,
			ReleaseDate = request.ReleaseDate
		};

		var created = await repository.CreateAsync(game, cancellationToken);
		var genre = await repository.GetGenreByIdAsync(created.GenreId, cancellationToken);
		await publisher.Publish(new GameCreatedNotification(
			created.Id, created.Name, created.GenreId, created.Price, created.ReleaseDate), cancellationToken);
		return new GameDto(created.Id, created.Name, genre?.Name ?? "", created.Price, created.ReleaseDate);
	}
}
