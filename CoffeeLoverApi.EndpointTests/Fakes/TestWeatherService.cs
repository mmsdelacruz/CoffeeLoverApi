using CoffeeLoverApi.Application.Abstractions;

namespace CoffeeLoverApi.EndpointTests.Fakes;

public sealed class TestWeatherService : IWeatherService
{
    private readonly double? _tempC;
    private int _calls;

    public TestWeatherService(double? tempC) => _tempC = tempC;

    public int Calls => _calls;

    public Task<double?> GetCurrentTemperatureCAsync(CancellationToken ct = default)
    {
        Interlocked.Increment(ref _calls);
        return Task.FromResult(_tempC);
    }
}