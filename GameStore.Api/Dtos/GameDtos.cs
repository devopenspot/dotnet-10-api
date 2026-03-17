namespace GameStore.Api.Dtos;

/// <summary>
/// Data transfer object for game information.
/// </summary>
/// <param name="Id">The unique identifier of the game.</param>
/// <param name="Name">The name of the game.</param>
/// <param name="Genre">The genre name of the game.</param>
/// <param name="Price">The price of the game.</param>
/// <param name="ReleaseDate">The release date of the game.</param>
public record GameDto(int Id, string Name, string Genre, decimal Price, DateOnly ReleaseDate);