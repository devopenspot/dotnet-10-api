using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Application;

/// <summary>
/// Data transfer object for creating a new game.
/// </summary>
/// <param name="Name">The name of the game.</param>
/// <param name="GenreId">The identifier of the genre.</param>
/// <param name="Price">The price of the game.</param>
/// <param name="ReleaseDate">The release date of the game.</param>
public record CreateGameDto(
	[Required][StringLength(50)] string Name,
	[Required] int GenreId,
	[Range(1, 100)] decimal Price, 
	DateOnly ReleaseDate
);