using GameStore.Api.Application;

namespace GameStore.Api.test.ObjectMothers;

public static class GameDtoObjectMother
{
    private const string DefaultName = "Test Game";
    private const decimal DefaultPrice = 29.99m;
    private const int DefaultGenreId = 1;
    private static readonly DateOnly DefaultReleaseDate = new(2023, 1, 1);

    public static GameDto CreateDto() => new(1, DefaultName, "Action", DefaultPrice, DefaultReleaseDate);

    public static GameDto WithName(string name) => new(1, name, "Action", DefaultPrice, DefaultReleaseDate);

    public static GameDto WithGenre(string genre) => new(1, DefaultName, genre, DefaultPrice, DefaultReleaseDate);

    public static GameDto WithPrice(decimal price) => new(1, DefaultName, "Action", price, DefaultReleaseDate);

    public static CreateGameDto CreateCreateDto() => new(DefaultName, DefaultGenreId, DefaultPrice, DefaultReleaseDate);

    public static CreateGameDto WithNameCreate(string name) => new(name, DefaultGenreId, DefaultPrice, DefaultReleaseDate);

    public static CreateGameDto WithGenreIdCreate(int genreId) => new(DefaultName, genreId, DefaultPrice, DefaultReleaseDate);

    public static CreateGameDto WithPriceCreate(decimal price) => new(DefaultName, DefaultGenreId, price, DefaultReleaseDate);

    public static UpdateGameDto CreateUpdateDto() => new(DefaultName, DefaultGenreId, DefaultPrice, DefaultReleaseDate);

    public static UpdateGameDto WithIdUpdate(int id) => new(DefaultName, DefaultGenreId, DefaultPrice, DefaultReleaseDate);

    public static UpdateGameDto WithNameUpdate(string name) => new(name, DefaultGenreId, DefaultPrice, DefaultReleaseDate);

    public static UpdateGameDto WithGenreIdUpdate(int genreId) => new(DefaultName, genreId, DefaultPrice, DefaultReleaseDate);

    public static UpdateGameDto WithPriceUpdate(decimal price) => new(DefaultName, DefaultGenreId, price, DefaultReleaseDate);
}
