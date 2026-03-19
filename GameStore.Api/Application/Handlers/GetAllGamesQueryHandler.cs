using GameStore.Api.Application.Cache;
using GameStore.Api.Application.Ports;
using GameStore.Api.Application.Queries;
using MediatR;

namespace GameStore.Api.Application.Handlers;

public class GetAllGamesQueryHandler(
	IGameRepository repository,
	IGameQueryService queryService) : IRequestHandler<GetAllGamesQuery, List<GameDto>>
{
	public async Task<List<GameDto>> Handle(GetAllGamesQuery request, CancellationToken cancellationToken)
	{
		var stopwatch = System.Diagnostics.Stopwatch.StartNew();
		var fromCache = false;
		try
		{
			var cached = await queryService.GetAllGamesAsync(cancellationToken);
			if (cached is not null)
			{
				fromCache = true;
				AppMetrics.CacheHits.Add(1);
				return cached;
			}

			var games = await repository.GetAllAsync(cancellationToken);
			var dtos = new List<GameDto>();
			foreach (var game in games)
			{
				var genre = await repository.GetGenreByIdAsync(game.GenreId, cancellationToken);
				dtos.Add(new GameDto(game.Id, game.Name, genre?.Name ?? "", game.Price, game.ReleaseDate));
			}

			await queryService.SetAllGamesAsync(dtos, cancellationToken);
			
			if (!fromCache)
			{
				AppMetrics.CacheMisses.Add(1);
			}
			
			return dtos;
		}
		finally
		{
			stopwatch.Stop();
			AppMetrics.QueryHandlerDuration.Record(stopwatch.Elapsed.TotalMilliseconds);
		}
	}
}
