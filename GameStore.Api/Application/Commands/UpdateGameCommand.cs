using GameStore.Api.Domain;
using MediatR;

namespace GameStore.Api.Application.Commands;

public record UpdateGameCommand(
	int Id,
	string Name,
	int GenreId,
	decimal Price,
	DateOnly ReleaseDate
) : IRequest<Game?>;
