using GameStore.Api.Application;

namespace GameStore.Api.test.ObjectMothers;

public static class GenreDtoObjectMother
{
    public static GenreDto Create() => new(1, "Action");

    public static GenreDto Action() => new(1, "Action");

    public static GenreDto Adventure() => new(2, "Adventure");

    public static GenreDto Strategy() => new(3, "Strategy");

    public static GenreDto WithId(int id) => new(id, "Action");

    public static GenreDto WithName(string name) => new(1, name);
}
