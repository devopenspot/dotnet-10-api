using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace GameStore.Api.Application.Cache;

public class RedisGameQueryService(IDistributedCache cache) : IGameQueryService
{
	private static readonly DistributedCacheEntryOptions Options = new()
	{
		AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
	};

	public async Task<List<GameDto>?> GetAllGamesAsync(CancellationToken ct = default)
	{
		var data = await cache.GetStringAsync("games:all", ct);
		return data is null ? null : JsonSerializer.Deserialize<List<GameDto>>(data);
	}

	public async Task<GameDto?> GetGameByIdAsync(int id, CancellationToken ct = default)
	{
		var data = await cache.GetStringAsync($"games:{id}", ct);
		return data is null ? null : JsonSerializer.Deserialize<GameDto>(data);
	}

	public async Task SetAllGamesAsync(List<GameDto> games, CancellationToken ct = default)
	{
		var data = JsonSerializer.Serialize(games);
		await cache.SetStringAsync("games:all", data, Options, ct);
	}

	public async Task SetGameAsync(GameDto game, CancellationToken ct = default)
	{
		var data = JsonSerializer.Serialize(game);
		await cache.SetStringAsync($"games:{game.Id}", data, Options, ct);
	}

	public async Task InvalidateAllGamesAsync(CancellationToken ct = default)
	{
		await cache.RemoveAsync("games:all", ct);
	}

	public async Task InvalidateGameAsync(int id, CancellationToken ct = default)
	{
		await cache.RemoveAsync($"games:{id}", ct);
	}
}
