using CoffeeLoverApi.EndpointTests.Fakes;
using System.Net;
using System.Net.Http.Json;

namespace CoffeeLoverApi.EndpointTests;

public sealed class BrewCoffeeEndpointTests
{
    [Fact]
    public async Task TempGreaterThan30_ReturnsIcedCoffeeMessage()
    {
        var clock = new FixedDateTimeProvider(new DateTimeOffset(2026, 5, 20, 10, 30, 0, TimeSpan.FromHours(8)));
        var repo = new InMemoryCounterRepo();
        var weather = new TestWeatherService(tempC: 31.0);

        await using var factory = new CoffeeApiFactory(clock, repo, weather);
        using var client = factory.CreateClient();

        var resp = await client.GetAsync("/brew-coffee");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

        var dto = await resp.Content.ReadFromJsonAsync<BrewDto>();
        Assert.NotNull(dto);
        Assert.Equal("Your refreshing iced coffee is ready", dto!.message);
        Assert.EndsWith("+0800", dto.prepared);

        Assert.Equal(1, weather.Calls);
    }

    [Fact]
    public async Task Temp30OrLess_ReturnsHotCoffeeMessage()
    {
        var clock = new FixedDateTimeProvider(new DateTimeOffset(2026, 5, 20, 10, 30, 0, TimeSpan.FromHours(8)));
        var repo = new InMemoryCounterRepo();
        var weather = new TestWeatherService(tempC: 30.0);

        await using var factory = new CoffeeApiFactory(clock, repo, weather);
        using var client = factory.CreateClient();

        var resp = await client.GetAsync("/brew-coffee");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

        var dto = await resp.Content.ReadFromJsonAsync<BrewDto>();
        Assert.NotNull(dto);
        Assert.Equal("Your piping hot coffee is ready", dto!.message);

        Assert.Equal(1, weather.Calls);
    }

    [Fact]
    public async Task EveryFifthCall_Returns503_EmptyBody_AndDoesNotCallWeatherOn5th()
    {
        var clock = new FixedDateTimeProvider(new DateTimeOffset(2026, 5, 20, 10, 30, 0, TimeSpan.FromHours(8)));
        var repo = new InMemoryCounterRepo();
        var weather = new TestWeatherService(tempC: 25.0);

        await using var factory = new CoffeeApiFactory(clock, repo, weather);
        using var client = factory.CreateClient();

        // Calls 1-4 -> 200 OK
        for (int i = 1; i <= 4; i++)
        {
            var ok = await client.GetAsync("/brew-coffee");
            Assert.Equal(HttpStatusCode.OK, ok.StatusCode);
        }

        // Call 5 -> 503 + empty body
        var fifth = await client.GetAsync("/brew-coffee");
        Assert.Equal(HttpStatusCode.ServiceUnavailable, fifth.StatusCode);
        Assert.Equal(string.Empty, await fifth.Content.ReadAsStringAsync());

        // Weather called only for the 4 successful requests (5th should short-circuit)
        Assert.Equal(4, weather.Calls);
    }

    [Fact]
    public async Task AprilFirst_Returns418_EmptyBody_AndNeverCallsWeather()
    {
        var clock = new FixedDateTimeProvider(new DateTimeOffset(2026, 4, 1, 9, 0, 0, TimeSpan.FromHours(8)));
        var repo = new InMemoryCounterRepo();
        var weather = new TestWeatherService(tempC: 40.0);

        await using var factory = new CoffeeApiFactory(clock, repo, weather);
        using var client = factory.CreateClient();

        // Multiple calls; all must be 418 with empty body
        for (int i = 0; i < 5; i++)
        {
            var r = await client.GetAsync("/brew-coffee");
            Assert.Equal((HttpStatusCode)418, r.StatusCode);
            Assert.Equal(string.Empty, await r.Content.ReadAsStringAsync());
        }

        // Should not call weather at all on April 1
        Assert.Equal(0, weather.Calls);
    }

    [Fact]
    public async Task WeatherUnavailable_FallsBackToHotCoffee()
    {
        var clock = new FixedDateTimeProvider(new DateTimeOffset(2026, 5, 20, 10, 30, 0, TimeSpan.FromHours(8)));
        var repo = new InMemoryCounterRepo();
        var weather = new TestWeatherService(tempC: null);

        await using var factory = new CoffeeApiFactory(clock, repo, weather);
        using var client = factory.CreateClient();

        var resp = await client.GetAsync("/brew-coffee");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);

        var dto = await resp.Content.ReadFromJsonAsync<BrewDto>();
        Assert.NotNull(dto);
        Assert.Equal("Your piping hot coffee is ready", dto!.message);

        Assert.Equal(1, weather.Calls);
    }

    private sealed record BrewDto(string message, string prepared);
}