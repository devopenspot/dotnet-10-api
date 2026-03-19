using MediatR;

namespace GameStore.Api.Application.Queries;

public record GetGameByIdQuery(int Id) : IRequest<GameDto?>;
