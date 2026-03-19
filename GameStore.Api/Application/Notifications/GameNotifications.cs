using MediatR;

namespace GameStore.Api.Application.Notifications;

public record GameCreatedNotification(int Id, string Name, int GenreId, decimal Price, DateOnly ReleaseDate) : INotification;

public record GameUpdatedNotification(int Id, string Name, int GenreId, decimal Price, DateOnly ReleaseDate) : INotification;

public record GameDeletedNotification(int Id) : INotification;
