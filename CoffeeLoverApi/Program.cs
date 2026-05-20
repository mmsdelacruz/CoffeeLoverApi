using CoffeeLoverApi.Application.Abstractions;
using CoffeeLoverApi.Application.Services;
using CoffeeLoverApi.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// existing DI
builder.Services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
builder.Services.AddSingleton<ICoffeeMachineRepository, InMemoryCoffeeMachineRepository>();
builder.Services.AddScoped<ICoffeeService, CoffeeService>();

// Weather configuration + typed HttpClient
builder.Services.Configure<OpenWeatherOptions>(builder.Configuration.GetSection("OpenWeather"));
builder.Services.AddHttpClient<IWeatherService, OpenWeatherService>(client =>
{
    client.BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/");
    client.Timeout = TimeSpan.FromSeconds(3);
});

var app = builder.Build();
app.MapControllers();
app.Run();

public partial class Program { }