using System.Net;
using System.Net.Http.Json;
using GameStore.Api.Dtos;
using GameStore.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GameStore.Api.test;

public class EndpointsIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public EndpointsIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetGames_ShouldReturnOk()
    {
        // Scaffold: Test GET /games returns OK
    }

    [Fact]
    public async Task GetGameById_ShouldReturnGame_WhenExists()
    {
        // Scaffold: Test GET /games/{id} returns game when exists
    }

    [Fact]
    public async Task GetGameById_ShouldReturnNotFound_WhenNotExists()
    {
        // Scaffold: Test GET /games/{id} returns 404 when not exists
    }

    [Fact]
    public async Task CreateGame_ShouldReturnCreated_WhenValid()
    {
        // Scaffold: Test POST /games creates game
    }

    [Fact]
    public async Task CreateGame_ShouldReturnBadRequest_WhenInvalid()
    {
        // Scaffold: Test POST /games returns 400 for invalid data
    }

    [Fact]
    public async Task UpdateGame_ShouldReturnOk_WhenExists()
    {
        // Scaffold: Test PUT /games/{id} updates game
    }

    [Fact]
    public async Task UpdateGame_ShouldReturnNotFound_WhenNotExists()
    {
        // Scaffold: Test PUT /games/{id} returns 404 when not exists
    }

    [Fact]
    public async Task DeleteGame_ShouldReturnNoContent_WhenExists()
    {
        // Scaffold: Test DELETE /games/{id} deletes game
    }

    [Fact]
    public async Task DeleteGame_ShouldReturnNotFound_WhenNotExists()
    {
        // Scaffold: Test DELETE /games/{id} returns 404 when not exists
    }
}