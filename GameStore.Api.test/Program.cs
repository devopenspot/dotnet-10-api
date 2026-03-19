using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GameStore.Api.Application;
using GameStore.Api.Application.Cache;
using GameStore.Api.Application.Commands;
using GameStore.Api.Application.Ports;
using GameStore.Api.Endpoints;
using GameStore.Api.Infrastructure;
using GameStore.Api.Infrastructure.Adapters;
using GameStore.Api.Domain;
using GameStore.Api.test;
using MediatR;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddValidation();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateGameCommand).Assembly));
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IGameQueryService, InMemoryGameQueryService>();
builder.Services.AddDbContext<GameStoreContext>(options => options.UseSqlite("Data Source=gamestore_test.db"));
builder.Services.AddDistributedMemoryCache();
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
	var context = scope.ServiceProvider.GetRequiredService<GameStoreContext>();
	context.Database.EnsureCreated();
	if (!context.Set<Genre>().Any())
	{
		context.Set<Genre>().AddRange(
			new Genre { Name = "Action" },
			new Genre { Name = "Adventure" },
			new Genre { Name = "RPG" },
			new Genre { Name = "Strategy" },
			new Genre { Name = "Simulation" }
		);
		context.SaveChanges();
	}

	if (!context.Set<Game>().Any())
	{
		context.Set<Game>().AddRange(
			new Game { Name = "The Legend of Zelda: Breath of the Wild", GenreId = 2, ReleaseDate = new (2017, 3, 3) },
			new Game { Name = "The Witcher 3: Wild Hunt", GenreId = 3, ReleaseDate = new (2015, 5, 19) },
			new Game { Name = "Civilization VI", GenreId = 4, ReleaseDate = new (2016, 10, 25) },
			new Game { Name = "Microsoft Flight Simulator", GenreId = 5, ReleaseDate = new (2020, 8, 4) },
			new Game { Name = "DOOM Eternal", GenreId = 1, ReleaseDate = new (2020, 3, 10) }
		);
		context.SaveChanges();
	}
}
app.MapGamesEndpoints();
app.Run();
