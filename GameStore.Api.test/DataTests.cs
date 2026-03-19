using GameStore.Api.Infrastructure;
using GameStore.Api.Domain;
using Microsoft.EntityFrameworkCore;
using GameStore.Api.test.ObjectMothers;

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
        var genre = GenreObjectMother.Create();
        context.Set<Genre>().Add(genre);
        Assert.Single(context.ChangeTracker.Entries<Genre>());

        var game = GameObjectMother.Create(g => g.GenreId = genre.Id);
        context.Set<Game>().Add(game);
        Assert.Single(context.ChangeTracker.Entries<Game>());
        Assert.Equal("Test Game", game.Name);
        Assert.Equal(29.99m, game.Price);
        Assert.Equal(new DateOnly(2023, 1, 1), game.ReleaseDate);
        Assert.Equal(genre.Id, game.GenreId);
    }

    [Fact]
    public async Task GameStoreContext_ShouldSaveAndRetrieveGenre()
    {
        using var context = CreateContext();
        var genre = GenreObjectMother.Adventure();
        context.Set<Genre>().Add(genre);
        Assert.Single(context.ChangeTracker.Entries<Genre>());
        Assert.Equal("Adventure", genre.Name);
    }
}