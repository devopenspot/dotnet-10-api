using GameStore.Api.Domain;
using GameStore.Api.test.ObjectMothers;

namespace GameStore.Api.test;

public class ModelsTests
{
    [Fact]
    public void Game_ShouldHaveRequiredProperties()
    {
        var game = GameObjectMother.Create();
        Assert.Equal(1, game.Id);
        Assert.Equal("Test Game", game.Name);
        Assert.Equal(1, game.GenreId);
        Assert.Equal(29.99m, game.Price);
        Assert.Equal(new DateOnly(2023, 1, 1), game.ReleaseDate);
    }

    [Fact]
    public void Genre_ShouldHaveRequiredProperties()
    {
        var genre = GenreObjectMother.Create();
        Assert.Equal(1, genre.Id);
        Assert.Equal("Action", genre.Name);
    }

    [Fact]
    public void Game_ShouldAllowSettingProperties()
    {
        var game = GameObjectMother.Create(g =>
        {
            g.Id = 2;
            g.Name = "Another Game";
            g.GenreId = 2;
            g.Price = 19.99m;
            g.ReleaseDate = new DateOnly(2024, 5, 10);
        });
        Assert.Equal(2, game.Id);
        Assert.Equal("Another Game", game.Name);
        Assert.Equal(2, game.GenreId);
        Assert.Equal(19.99m, game.Price);
        Assert.Equal(new DateOnly(2024, 5, 10), game.ReleaseDate);
    }

    [Fact]
    public void Genre_ShouldAllowSettingProperties()
    {
        var genre = GenreObjectMother.Create(g =>
        {
            g.Id = 2;
            g.Name = "Adventure";
        });
        Assert.Equal(2, genre.Id);
        Assert.Equal("Adventure", genre.Name);
    }
}