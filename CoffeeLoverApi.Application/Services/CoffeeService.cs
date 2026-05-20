using CoffeeLoverApi.Application.Abstractions;
using CoffeeLoverApi.Domain;

namespace CoffeeLoverApi.Application.Services;

public sealed class CoffeeService : ICoffeeService
{
    private readonly IDateTimeProvider _clock;
    private readonly ICoffeeMachineRepository _repo;
    private readonly IWeatherService _weather;

    public CoffeeService(IDateTimeProvider clock, ICoffeeMachineRepository repo, IWeatherService weather)
    {
        _clock = clock;
        _repo = repo;
        _weather = weather;
    }

    public async Task<BrewResult> BrewCoffeeAsync(CancellationToken ct = default)
    {
        var now = _clock.Now();

        // #3: April 1st => 418, empty body
        if (now.Month == 4 && now.Day == 1)
            return new BrewResult(BrewOutcome.Teapot);

        // #2: Every fifth call => 503, empty body
        var callNumber = _repo.IncrementAndGetBrewCount();
        if (callNumber % 5 == 0)
            return new BrewResult(BrewOutcome.OutOfCoffee);

        // #1: Otherwise 200 with JSON, but message depends on temperature > 30°C
        var tempC = await _weather.GetCurrentTemperatureCAsync(ct);
        var message = (tempC is not null && tempC > 30.0)
            ? "Your refreshing iced coffee is ready"
            : "Your piping hot coffee is ready";

        return new BrewResult(
            BrewOutcome.Ok,
            new BrewCoffeeResponse(message, ToIso8601BasicOffset(now))
        );
    }

    private static string ToIso8601BasicOffset(DateTimeOffset dto)
    {
        // "2026-05-20T10:30:00+08:00" -> "2026-05-20T10:30:00+0800"
        var s = dto.ToString("yyyy-MM-dd'T'HH:mm:sszzz");
        return s[..^3] + s[^2..];
    }
}