using GameStore.Api.Application.Commands;
using GameStore.Api.Application.Ports;
using GameStore.Api.Endpoints;
using GameStore.Api.Infrastructure;
using GameStore.Api.Infrastructure.Adapters;
using MediatR;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidation();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateGameCommand).Assembly));
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.GameStoreDatabase();
var app = builder.Build();
app.MigrateDatabase();
app.MapGamesEndpoints();
app.Run();
