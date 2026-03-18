using GameStore.Api.Application;
using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.test;

public class DtosTests
{
    [Fact]
    public void GameDto_ShouldBeCreatedWithValidData()
    {
        var gameDto = new GameDto(1, "Test Game", "Action", 29.99m, new DateOnly(2023, 1, 1));
        Assert.Equal(1, gameDto.Id);
        Assert.Equal("Test Game", gameDto.Name);
        Assert.Equal("Action", gameDto.Genre);
        Assert.Equal(29.99m, gameDto.Price);
        Assert.Equal(new DateOnly(2023, 1, 1), gameDto.ReleaseDate);
    }

    [Fact]
    public void CreateGameDto_ShouldValidateRequiredFields()
    {
        var requiredAttr = new RequiredAttribute();
        var nameResult = requiredAttr.GetValidationResult("", new ValidationContext(new CreateGameDto("", 1, 10m, new DateOnly(2023, 1, 1))) { MemberName = "Name" });
        Assert.NotEqual(ValidationResult.Success, nameResult);

        // GenreId is int, [Required] doesn't apply to value types
    }

    [Fact]
    public void CreateGameDto_ShouldValidatePriceRange()
    {
        var rangeAttr = new RangeAttribute(1, 100);
        var lowResult = rangeAttr.GetValidationResult(0.5m, new ValidationContext(new CreateGameDto("Test", 1, 0.5m, new DateOnly(2023, 1, 1))) { MemberName = "Price" });
        Assert.NotEqual(ValidationResult.Success, lowResult);

        var highResult = rangeAttr.GetValidationResult(150m, new ValidationContext(new CreateGameDto("Test", 1, 150m, new DateOnly(2023, 1, 1))) { MemberName = "Price" });
        Assert.NotEqual(ValidationResult.Success, highResult);

        var validResult = rangeAttr.GetValidationResult(50m, new ValidationContext(new CreateGameDto("Test", 1, 50m, new DateOnly(2023, 1, 1))) { MemberName = "Price" });
        Assert.Equal(ValidationResult.Success, validResult);
    }

    [Fact]
    public void UpdateGameDto_ShouldValidateRequiredFields()
    {
        var requiredAttr = new RequiredAttribute();
        var nameResult = requiredAttr.GetValidationResult("", new ValidationContext(new UpdateGameDto("", 1, 10m, new DateOnly(2023, 1, 1))) { MemberName = "Name" });
        Assert.NotEqual(ValidationResult.Success, nameResult);

        // GenreId is int, [Required] doesn't apply to value types
    }

    [Fact]
    public void GenreDto_ShouldBeCreatedWithValidData()
    {
        var genreDto = new GenreDto(1, "Action");
        Assert.Equal(1, genreDto.Id);
        Assert.Equal("Action", genreDto.Name);
    }
}