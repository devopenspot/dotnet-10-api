using GameStore.Api.Domain;

namespace GameStore.Api.test.ObjectMothers;

public static class GameObjectMother
{
    private const string DefaultName = "Test Game";
    private const decimal DefaultPrice = 29.99m;
    private const int DefaultGenreId = 1;
    private static readonly DateOnly DefaultReleaseDate = new(2023, 1, 1);

    public static Game Create() => new()
    {
        Id = 1,
        Name = DefaultName,
        GenreId = DefaultGenreId,
        Price = DefaultPrice,
        ReleaseDate = DefaultReleaseDate
    };

    public static Game Create(Action<Game> configure)
    {
        var game = Create();
        configure(game);
        return game;
    }

    public static Game WithGenre(Genre genre) => Create(g => g.GenreId = genre.Id);

    public static Game ExpensiveGame() => Create(g => g.Price = 99.99m);

    public static Game CheapGame() => Create(g => g.Price = 9.99m);

    public static Game OldReleaseDate() => Create(g => g.ReleaseDate = new DateOnly(2000, 1, 1));

    public static Game NewRelease() => Create(g => g.ReleaseDate = DateOnly.FromDateTime(DateTime.Today));

    public static Game WithId(int id) => Create(g => g.Id = id);

    public static Game WithName(string name) => Create(g => g.Name = name);

    public static Game WithPrice(decimal price) => Create(g => g.Price = price);
}
