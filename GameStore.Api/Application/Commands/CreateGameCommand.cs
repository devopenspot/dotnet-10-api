using MediatR;

namespace GameStore.Api.Application.Commands;

public record CreateGameCommand(
	string Name,
	int GenreId,
	decimal Price,
	DateOnly ReleaseDate
) : IRequest<GameDto>;
