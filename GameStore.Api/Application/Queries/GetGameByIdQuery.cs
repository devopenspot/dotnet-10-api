using GameStore.Api.Domain;
using MediatR;

namespace GameStore.Api.Application.Queries;

public record GetGameByIdQuery(int Id) : IRequest<Game?>;
