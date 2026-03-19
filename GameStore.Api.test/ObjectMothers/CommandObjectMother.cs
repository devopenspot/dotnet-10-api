using GameStore.Api.Application.Commands;

namespace GameStore.Api.test.ObjectMothers;

public static class CommandObjectMother
{
    private const string DefaultName = "Test Game";
    private const decimal DefaultPrice = 29.99m;
    private const int DefaultGenreId = 1;
    private static readonly DateOnly DefaultReleaseDate = new(2023, 1, 1);

    public static CreateGameCommand CreateCreateCommand() => new(DefaultName, DefaultGenreId, DefaultPrice, DefaultReleaseDate);

    public static CreateGameCommand WithNameCreateCommand(string name) => new(name, DefaultGenreId, DefaultPrice, DefaultReleaseDate);

    public static CreateGameCommand WithGenreIdCreateCommand(int genreId) => new(DefaultName, genreId, DefaultPrice, DefaultReleaseDate);

    public static CreateGameCommand WithPriceCreateCommand(decimal price) => new(DefaultName, DefaultGenreId, price, DefaultReleaseDate);

    public static UpdateGameCommand CreateUpdateCommand() => new(1, DefaultName, DefaultGenreId, DefaultPrice, DefaultReleaseDate);

    public static UpdateGameCommand WithIdUpdateCommand(int id) => new(id, DefaultName, DefaultGenreId, DefaultPrice, DefaultReleaseDate);

    public static UpdateGameCommand WithNameUpdateCommand(string name) => new(1, name, DefaultGenreId, DefaultPrice, DefaultReleaseDate);

    public static UpdateGameCommand WithGenreIdUpdateCommand(int genreId) => new(1, DefaultName, genreId, DefaultPrice, DefaultReleaseDate);

    public static UpdateGameCommand WithPriceUpdateCommand(decimal price) => new(1, DefaultName, DefaultGenreId, price, DefaultReleaseDate);

    public static DeleteGameCommand CreateDeleteCommand() => new(1);

    public static DeleteGameCommand WithIdDeleteCommand(int id) => new(id);
}
