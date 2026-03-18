using GameStore.Api.Endpoints;
using GameStore.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidation();
builder.GameStoreDatabase();
var app = builder.Build();
app.MigrateDatabase();
app.MapGamesEndpoints();
app.Run();
