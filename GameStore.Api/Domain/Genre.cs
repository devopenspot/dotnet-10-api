namespace GameStore.Api.Domain;

/// <summary>
/// Represents a genre in the GameStore.
/// </summary>
public class Genre
{
	/// <summary>
	/// Gets or sets the unique identifier for the genre.
	/// </summary>
	public int Id { get; set; }

	/// <summary>
	/// Gets or sets the name of the genre.
	/// </summary>
	public required string Name { get; set; }
}