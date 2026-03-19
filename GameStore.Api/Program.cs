using GameStore.Api.Application.Cache;
using GameStore.Api.Application.Commands;
using GameStore.Api.Application.Ports;
using GameStore.Api.Endpoints;
using GameStore.Api.Infrastructure;
using GameStore.Api.Infrastructure.Adapters;
using MediatR;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidation();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateGameCommand).Assembly));
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IGameQueryService, RedisGameQueryService>();

var redisConnection = builder.Configuration.GetConnectionString("RedisConnection");
if (!string.IsNullOrEmpty(redisConnection))
{
	builder.Services.AddStackExchangeRedisCache(options =>
	{
		options.Configuration = redisConnection;
		options.InstanceName = "GameStore:";
	});
}
else
{
	builder.Services.AddDistributedMemoryCache();
}

builder.GameStoreDatabase();
var app = builder.Build();
app.MigrateDatabase();
app.MapGamesEndpoints();
app.Run();
