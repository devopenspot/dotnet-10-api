using GameStore.Api.Application.Commands;
using GameStore.Api.Application.Ports;
using GameStore.Api.Domain;
using MediatR;

namespace GameStore.Api.Application.Handlers;

public class UpdateGameCommandHandler(IGameRepository repository) : IRequestHandler<UpdateGameCommand, Game?>
{
	public async Task<Game?> Handle(UpdateGameCommand request, CancellationToken cancellationToken)
	{
		var game = new Game
		{
			Id = request.Id,
			Name = request.Name,
			GenreId = request.GenreId,
			Price = request.Price,
			ReleaseDate = request.ReleaseDate
		};

		return await repository.UpdateAsync(game, cancellationToken);
	}
}
