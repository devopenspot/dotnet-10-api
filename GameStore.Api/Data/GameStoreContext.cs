using Microsoft.EntityFrameworkCore;
using GameStore.Api.Models;

namespace GameStore.Api.Data;

/// <summary>
/// Database context for the GameStore application.
/// </summary>
/// <param name="options">The options for configuring the context.</param>
public class GameStoreContext(DbContextOptions<GameStoreContext> options) : DbContext(options)
{
	/// <summary>
	/// Gets the DbSet for Games.
	/// </summary>
	DbSet<Game> Games => Set<Game>();

	/// <summary>
	/// Gets the DbSet for Genres.
	/// </summary>
	DbSet<Genre> Genres => Set<Genre>();
}