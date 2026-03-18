using GameStore.Api.Application.Commands;
using GameStore.Api.Endpoints;
using GameStore.Api.Infrastructure;
using MediatR;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidation();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateGameCommand).Assembly));
builder.GameStoreDatabase();
var app = builder.Build();
app.MigrateDatabase();
app.MapGamesEndpoints();
app.Run();
