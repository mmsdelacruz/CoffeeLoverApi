namespace CoffeeLoverApi.Application.Abstractions
{
    public interface IWeatherService
    {
        /// <summary>
        /// Returns current temperature in Celsius if available; null if unavailable.
        /// </summary>
        Task<double?> GetCurrentTemperatureCAsync(CancellationToken ct = default);
    }
}
