using CoffeeLoverApi.Application.Abstractions;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace CoffeeLoverApi.Infrastructure;

public sealed class OpenWeatherService : IWeatherService
{
    private readonly HttpClient _http;
    private readonly OpenWeatherOptions _options;

    public OpenWeatherService(HttpClient http, IOptions<OpenWeatherOptions> options)
    {
        _http = http;
        _options = options.Value;
    }

    public async Task<double?> GetCurrentTemperatureCAsync(CancellationToken ct = default)
    {
        // https://api.openweathermap.org/data/2.5/weather?lat=...&lon=...&appid=...&units=metric [2](https://openweathermap.org/api/current)
        var url = $"weather?lat={_options.Lat}&lon={_options.Lon}&appid={_options.ApiKey}&units=metric";

        try
        {
            var dto = await _http.GetFromJsonAsync<OpenWeatherDto>(url, cancellationToken: ct);
            return dto?.main?.temp;
        }
        catch
        {
            // Fail open: if weather is down, we still serve hot coffee
            return null;
        }
    }

    private sealed class OpenWeatherDto
    {
        public MainDto? main { get; set; }
    }

    private sealed class MainDto
    {
        public double temp { get; set; }
    }
}