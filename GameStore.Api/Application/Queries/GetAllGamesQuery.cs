using GameStore.Api.Domain;
using MediatR;

namespace GameStore.Api.Application.Queries;

public record GetAllGamesQuery : IRequest<List<Game>>;
