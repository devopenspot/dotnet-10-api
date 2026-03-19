using GameStore.Api.Domain;

namespace GameStore.Api.test.ObjectMothers;

public static class GenreObjectMother
{
    private static readonly DateOnly DefaultReleaseDate = new(2023, 1, 1);

    public static Genre Create() => new()
    {
        Id = 1,
        Name = "Action"
    };

    public static Genre Create(Action<Genre> configure)
    {
        var genre = Create();
        configure(genre);
        return genre;
    }

    public static Genre Action() => Create(g => g.Name = "Action");

    public static Genre Adventure() => Create(g => g.Name = "Adventure");

    public static Genre Strategy() => Create(g => g.Name = "Strategy");

    public static Genre Rpg() => Create(g => g.Name = "RPG");

    public static Genre Sports() => Create(g => g.Name = "Sports");
}
