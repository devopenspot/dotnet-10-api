namespace GameStore.Api.Application.Cache;

public interface IGameQueryService
{
	Task<List<GameDto>?> GetAllGamesAsync(CancellationToken ct = default);
	Task<GameDto?> GetGameByIdAsync(int id, CancellationToken ct = default);
	Task SetAllGamesAsync(List<GameDto> games, CancellationToken ct = default);
	Task SetGameAsync(GameDto game, CancellationToken ct = default);
	Task InvalidateAllGamesAsync(CancellationToken ct = default);
	Task InvalidateGameAsync(int id, CancellationToken ct = default);
}
