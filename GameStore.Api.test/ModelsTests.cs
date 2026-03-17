using GameStore.Api.Models;

namespace GameStore.Api.test;

public class ModelsTests
{
    [Fact]
    public void Game_ShouldHaveRequiredProperties()
    {
        var game = new Game
        {
            Id = 1,
            Name = "Test Game",
            GenreId = 1,
            Price = 29.99m,
            ReleaseDate = new DateOnly(2023, 1, 1)
        };
        Assert.Equal(1, game.Id);
        Assert.Equal("Test Game", game.Name);
        Assert.Equal(1, game.GenreId);
        Assert.Equal(29.99m, game.Price);
        Assert.Equal(new DateOnly(2023, 1, 1), game.ReleaseDate);
    }

    [Fact]
    public void Genre_ShouldHaveRequiredProperties()
    {
        var genre = new Genre
        {
            Id = 1,
            Name = "Action"
        };
        Assert.Equal(1, genre.Id);
        Assert.Equal("Action", genre.Name);
    }

    [Fact]
    public void Game_ShouldAllowSettingProperties()
    {
        var game = new Game
        {
            Id = 2,
            Name = "Another Game",
            GenreId = 2,
            Price = 19.99m,
            ReleaseDate = new DateOnly(2024, 5, 10)
        };
        Assert.Equal(2, game.Id);
        Assert.Equal("Another Game", game.Name);
        Assert.Equal(2, game.GenreId);
        Assert.Equal(19.99m, game.Price);
        Assert.Equal(new DateOnly(2024, 5, 10), game.ReleaseDate);
    }

    [Fact]
    public void Genre_ShouldAllowSettingProperties()
    {
        var genre = new Genre
        {
            Id = 2,
            Name = "Adventure"
        };
        Assert.Equal(2, genre.Id);
        Assert.Equal("Adventure", genre.Name);
    }
}