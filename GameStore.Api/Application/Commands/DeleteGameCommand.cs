using MediatR;

namespace GameStore.Api.Application.Commands;

public record DeleteGameCommand(int Id) : IRequest<bool>;
