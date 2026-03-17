using Microsoft.EntityFrameworkCore;
using GameStore.Api.Models;

namespace GameStore.Api.Data;
public class GameStoreContext(DbContextOptions<GameStoreContext> options) : DbContext(options)
{
	DbSet<Game> Games => Set<Game>();
	DbSet<Genre> Genres => Set<Genre>();
}