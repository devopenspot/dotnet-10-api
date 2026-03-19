using System.Diagnostics.Metrics;

namespace GameStore.Api.Application;

public static class AppMetrics
{
    public const string MeterName = "GameStore.Api";
    
    private static readonly Meter Meter = new(MeterName, "1.0.0");

    public static readonly Counter<long> GamesCreated = Meter.CreateCounter<long>(
        "gamestore.games.created",
        "games",
        "Number of games created");

    public static readonly Counter<long> GamesUpdated = Meter.CreateCounter<long>(
        "gamestore.games.updated",
        "games",
        "Number of games updated");

    public static readonly Counter<long> GamesDeleted = Meter.CreateCounter<long>(
        "gamestore.games.deleted",
        "games",
        "Number of games deleted");

    public static readonly Counter<long> CacheHits = Meter.CreateCounter<long>(
        "gamestore.cache.hits",
        "hits",
        "Number of cache hits");

    public static readonly Counter<long> CacheMisses = Meter.CreateCounter<long>(
        "gamestore.cache.misses",
        "misses",
        "Number of cache misses");

    public static readonly Histogram<double> QueryDuration = Meter.CreateHistogram<double>(
        "gamestore.query.duration",
        "ms",
        "Duration of database queries in milliseconds");

    public static readonly Histogram<double> CommandHandlerDuration = Meter.CreateHistogram<double>(
        "gamestore.command.duration",
        "ms",
        "Duration of command handlers in milliseconds");

    public static readonly Histogram<double> QueryHandlerDuration = Meter.CreateHistogram<double>(
        "gamestore.query_handler.duration",
        "ms",
        "Duration of query handlers in milliseconds");
}
