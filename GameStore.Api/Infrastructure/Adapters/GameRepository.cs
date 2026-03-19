using GameStore.Api.Application.Ports;
using GameStore.Api.Domain;
using GameStore.Api.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Infrastructure.Adapters;

public class GameRepository(GameStoreContext context) : IGameRepository
{
	public async Task<List<Game>> GetAllAsync(CancellationToken ct = default)
	{
		return await context.Set<Game>().AsNoTracking().ToListAsync(ct);
	}

	public async Task<Game?> GetByIdAsync(int id, CancellationToken ct = default)
	{
		return await context.Set<Game>().FindAsync([id], ct);
	}

	public async Task<Game> CreateAsync(Game game, CancellationToken ct = default)
	{
		context.Set<Game>().Add(game);
		await context.SaveChangesAsync(ct);
		return game;
	}

	public async Task<Game?> UpdateAsync(Game game, CancellationToken ct = default)
	{
		var existing = await context.Set<Game>().FindAsync([game.Id], ct);
		if (existing is null)
		{
			return null;
		}

		existing.Name = game.Name;
		existing.GenreId = game.GenreId;
		existing.Price = game.Price;
		existing.ReleaseDate = game.ReleaseDate;

		await context.SaveChangesAsync(ct);
		return existing;
	}

	public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
	{
		var game = await context.Set<Game>().FindAsync([id], ct);
		if (game is null)
		{
			return false;
		}

		context.Set<Game>().Remove(game);
		await context.SaveChangesAsync(ct);
		return true;
	}

	public async Task<Genre?> GetGenreByIdAsync(int id, CancellationToken ct = default)
	{
		return await context.Set<Genre>().FindAsync([id], ct);
	}
}
