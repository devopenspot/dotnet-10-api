using GameStore.Api.Application.Cache;
using GameStore.Api.Application.Ports;
using GameStore.Api.Application.Queries;
using MediatR;

namespace GameStore.Api.Application.Handlers;

public class GetGameByIdQueryHandler(
	IGameRepository repository,
	IGameQueryService queryService) : IRequestHandler<GetGameByIdQuery, GameDto?>
{
	public async Task<GameDto?> Handle(GetGameByIdQuery request, CancellationToken cancellationToken)
	{
		var cached = await queryService.GetGameByIdAsync(request.Id, cancellationToken);
		if (cached is not null)
		{
			return cached;
		}

		var game = await repository.GetByIdAsync(request.Id, cancellationToken);
		if (game is null)
		{
			return null;
		}

		var genre = await repository.GetGenreByIdAsync(game.GenreId, cancellationToken);
		var dto = new GameDto(game.Id, game.Name, genre?.Name ?? "", game.Price, game.ReleaseDate);
		await queryService.SetGameAsync(dto, cancellationToken);
		return dto;
	}
}
