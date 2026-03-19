using GameStore.Api.Application.Commands;
using GameStore.Api.Application.Ports;
using GameStore.Api.Domain;
using MediatR;

namespace GameStore.Api.Application.Handlers;

public class CreateGameCommandHandler(IGameRepository repository) : IRequestHandler<CreateGameCommand, Game>
{
	public async Task<Game> Handle(CreateGameCommand request, CancellationToken cancellationToken)
	{
		var game = new Game
		{
			Name = request.Name,
			GenreId = request.GenreId,
			Price = request.Price,
			ReleaseDate = request.ReleaseDate
		};

		return await repository.CreateAsync(game, cancellationToken);
	}
}
