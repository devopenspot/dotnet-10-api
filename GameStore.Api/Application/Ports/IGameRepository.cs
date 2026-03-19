using GameStore.Api.Domain;

namespace GameStore.Api.Application.Ports;

public interface IGameRepository
{
	Task<List<Game>> GetAllAsync(CancellationToken ct = default);
	Task<Game?> GetByIdAsync(int id, CancellationToken ct = default);
	Task<Game> CreateAsync(Game game, CancellationToken ct = default);
	Task<Game?> UpdateAsync(Game game, CancellationToken ct = default);
	Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
