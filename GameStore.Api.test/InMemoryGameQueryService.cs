using GameStore.Api.Application;
using GameStore.Api.Application.Cache;

namespace GameStore.Api.test;

public class InMemoryGameQueryService : IGameQueryService
{
	private readonly Dictionary<string, object?> _cache = new();

	public Task<List<GameDto>?> GetAllGamesAsync(CancellationToken ct = default) =>
		Task.FromResult(_cache.TryGetValue("games:all", out var v) ? v as List<GameDto> : null);

	public Task<GameDto?> GetGameByIdAsync(int id, CancellationToken ct = default) =>
		Task.FromResult(_cache.TryGetValue($"games:{id}", out var v) ? v as GameDto : null);

	public Task SetAllGamesAsync(List<GameDto> games, CancellationToken ct = default)
	{
		_cache["games:all"] = games;
		return Task.CompletedTask;
	}

	public Task SetGameAsync(GameDto game, CancellationToken ct = default)
	{
		_cache[$"games:{game.Id}"] = game;
		return Task.CompletedTask;
	}

	public Task InvalidateAllGamesAsync(CancellationToken ct = default)
	{
		_cache.Remove("games:all");
		return Task.CompletedTask;
	}

	public Task InvalidateGameAsync(int id, CancellationToken ct = default)
	{
		_cache.Remove($"games:{id}");
		return Task.CompletedTask;
	}
}
