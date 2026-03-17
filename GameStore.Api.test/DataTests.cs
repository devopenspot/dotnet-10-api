using GameStore.Api.Data;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.test;

public class DataTests
{
    private GameStoreContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<GameStoreContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;
        var context = new GameStoreContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public void GameStoreContext_ShouldCreateGamesDbSet()
    {
        using var context = CreateContext();
        Assert.NotNull(context.Set<Game>());
    }

    [Fact]
    public void GameStoreContext_ShouldCreateGenresDbSet()
    {
        using var context = CreateContext();
        Assert.NotNull(context.Set<Genre>());
    }

    [Fact]
    public void GameStoreContext_ShouldConfigureCorrectly()
    {
        using var context = CreateContext();
        Assert.NotNull(context);
        Assert.NotNull(context.Set<Game>());
        Assert.NotNull(context.Set<Genre>());
    }

    [Fact]
    public async Task GameStoreContext_ShouldSaveAndRetrieveGame()
    {
        using var context = CreateContext();
        var genre = new Genre { Name = "Action" };
        context.Set<Genre>().Add(genre);
        Assert.Equal(1, context.ChangeTracker.Entries<Genre>().Count());

        var game = new Game
        {
            Name = "Test Game",
            GenreId = genre.Id,
            Price = 29.99m,
            ReleaseDate = new DateOnly(2023, 1, 1)
        };
        context.Set<Game>().Add(game);
        Assert.Equal(1, context.ChangeTracker.Entries<Game>().Count());
        Assert.Equal("Test Game", game.Name);
        Assert.Equal(29.99m, game.Price);
        Assert.Equal(new DateOnly(2023, 1, 1), game.ReleaseDate);
        Assert.Equal(genre.Id, game.GenreId);
    }

    [Fact]
    public async Task GameStoreContext_ShouldSaveAndRetrieveGenre()
    {
        using var context = CreateContext();
        var genre = new Genre { Name = "Adventure" };
        context.Set<Genre>().Add(genre);
        Assert.Equal(1, context.ChangeTracker.Entries<Genre>().Count());
        Assert.Equal("Adventure", genre.Name);
    }
}