using MediatR;

namespace GameStore.Api.Application.Queries;

public record GetAllGamesQuery : IRequest<List<GameDto>>;
