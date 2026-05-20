using CoffeeLoverApi.EndpointTests.Fakes;
using System.Net;
using System.Net.Http.Json;

namespace CoffeeLoverApi.EndpointTests;

public sealed class BrewCoffeeEndpointTests
{
    [Fact]
    public async Task BrewCoffee_NormalDay_Returns200_WithJsonBody()
    {
        var clock = new FixedDateTimeProvider(new DateTimeOffset(2026, 5, 20, 10, 30, 0, TimeSpan.FromHours(8)));
        var repo = new InMemoryCounterRepo();

        await using var factory = new CoffeeApiFactory(clock, repo);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/brew-coffee");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<BrewCoffeeDto>();
        Assert.NotNull(payload);
        Assert.Equal("Your piping hot coffee is ready", payload!.message);
        Assert.EndsWith("+0800", payload.prepared); // offset without colon
        Assert.Contains("2026-05-20T10:30:00", payload.prepared);
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task BrewCoffee_EveryFifthCall_Returns503_EmptyBody()
    {
        var clock = new FixedDateTimeProvider(new DateTimeOffset(2026, 5, 20, 10, 30, 0, TimeSpan.FromHours(8)));
        var repo = new InMemoryCounterRepo();

        await using var factory = new CoffeeApiFactory(clock, repo);
        using var client = factory.CreateClient();

        // first 4 => OK
        for (int i = 0; i < 4; i++)
        {
            var r = await client.GetAsync("/brew-coffee");
            Assert.Equal(HttpStatusCode.OK, r.StatusCode);
        }

        // 5th => 503 with empty body
        var fifth = await client.GetAsync("/brew-coffee");
        Assert.Equal(HttpStatusCode.ServiceUnavailable, fifth.StatusCode);
        Assert.Equal(string.Empty, await fifth.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task BrewCoffee_AprilFirst_Returns418_EmptyBody_EveryTime()
    {
        var clock = new FixedDateTimeProvider(new DateTimeOffset(2026, 4, 1, 9, 0, 0, TimeSpan.FromHours(8)));
        var repo = new InMemoryCounterRepo();

        await using var factory = new CoffeeApiFactory(clock, repo);
        using var client = factory.CreateClient();

        var r1 = await client.GetAsync("/brew-coffee");
        var r2 = await client.GetAsync("/brew-coffee");

        Assert.Equal((HttpStatusCode)418, r1.StatusCode);
        Assert.Equal((HttpStatusCode)418, r2.StatusCode);

        Assert.Equal(string.Empty, await r1.Content.ReadAsStringAsync());
        Assert.Equal(string.Empty, await r2.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Calls_Sequence_Should_Return_503_OnEveryFifth()
    {
        var clock = new FixedDateTimeProvider(new DateTimeOffset(2026, 5, 20, 10, 30, 0, TimeSpan.FromHours(8)));
        var repo = new InMemoryCounterRepo();

        await using var factory = new CoffeeApiFactory(clock, repo);
        using var client = factory.CreateClient();

        for (int i = 1; i <= 10; i++)
        {
            var response = await client.GetAsync("/brew-coffee");

            if (i % 5 == 0)
                Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
            else
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    [Fact]
    public async Task AprilFirst_ShouldOverrideFifthCallRule()
    {
        var clock = new FixedDateTimeProvider(new DateTimeOffset(2026, 4, 1, 10, 0, 0, TimeSpan.Zero));
        var repo = new InMemoryCounterRepo();
        await using var factory = new CoffeeApiFactory(clock, repo);
        var client = factory.CreateClient();

        for (int i = 0; i < 5; i++)
        {
            var response = await client.GetAsync("/brew-coffee");

            Assert.Equal((HttpStatusCode)418, response.StatusCode);
        }
    }

    [Fact]
    public async Task ConcurrentCalls_ShouldStillRespectFifthRule()
    {
        var clock = new FixedDateTimeProvider(new DateTimeOffset(2026, 5, 20, 10, 30, 0, TimeSpan.FromHours(8)));
        var repo = new InMemoryCounterRepo();

        await using var factory = new CoffeeApiFactory(clock, repo);
        using var client = factory.CreateClient();

        var tasks = Enumerable.Range(0, 20)
            .Select(_ => client.GetAsync("/brew-coffee"));

        var responses = await Task.WhenAll(tasks);

        var count503 = responses.Count(r => r.StatusCode == HttpStatusCode.ServiceUnavailable);

        Assert.True(count503 >= 4); // approx every 5 calls
    }

    [Fact]
    public async Task SwaggerJson_IsServed_InDevelopment()
    {
        var clock = new FixedDateTimeProvider(new DateTimeOffset(2026, 5, 20, 10, 30, 0, TimeSpan.FromHours(8)));
        var repo = new InMemoryCounterRepo();

        await using var factory = new CoffeeApiFactory(clock, repo);
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("\"openapi\"", json);
        Assert.Contains("\"paths\"", json);
    }

    private sealed record BrewCoffeeDto(string message, string prepared);
}