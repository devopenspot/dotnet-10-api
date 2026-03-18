namespace GameStore.Api.Application;

/// <summary>
/// Data transfer object for genre information.
/// </summary>
/// <param name="Id">The unique identifier of the genre.</param>
/// <param name="Name">The name of the genre.</param>
public record GenreDto(int Id, string Name);