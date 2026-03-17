using GameStore.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.test;

public class DataTests
{
    [Fact]
    public void GameStoreContext_ShouldCreateGamesDbSet()
    {
        // Scaffold: Test that context has Games DbSet
    }

    [Fact]
    public void GameStoreContext_ShouldCreateGenresDbSet()
    {
        // Scaffold: Test that context has Genres DbSet
    }

    [Fact]
    public void GameStoreContext_ShouldConfigureCorrectly()
    {
        // Scaffold: Test context configuration
    }

    [Fact]
    public async Task GameStoreContext_ShouldSaveAndRetrieveGame()
    {
        // Scaffold: Test saving and retrieving a game
    }

    [Fact]
    public async Task GameStoreContext_ShouldSaveAndRetrieveGenre()
    {
        // Scaffold: Test saving and retrieving a genre
    }
}