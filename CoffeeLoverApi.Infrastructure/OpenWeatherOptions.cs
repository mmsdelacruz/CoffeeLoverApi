namespace CoffeeLoverApi.Infrastructure
{
    public sealed class OpenWeatherOptions
    {
        public string ApiKey { get; set; } = "";
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}
