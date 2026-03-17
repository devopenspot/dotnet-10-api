namespace GameStore.Api.Models;

/// <summary>
/// Represents a game in the GameStore.
/// </summary>
public class Game
{
	/// <summary>
	/// Gets or sets the unique identifier for the game.
	/// </summary>
	public int Id { get; set; }

	/// <summary>
	/// Gets or sets the name of the game.
	/// </summary>
	public required string Name { get; set; }

	/// <summary>
	/// Gets or sets the genre of the game.
	/// </summary>
	public Genre? Genre { get; set; }

	/// <summary>
	/// Gets or sets the genre identifier.
	/// </summary>
	public int GenreId { get; set; }

	/// <summary>
	/// Gets or sets the price of the game.
	/// </summary>
	public decimal Price { get; set; }

	/// <summary>
	/// Gets or sets the release date of the game.
	/// </summary>
	public DateOnly ReleaseDate { get; set; }
}