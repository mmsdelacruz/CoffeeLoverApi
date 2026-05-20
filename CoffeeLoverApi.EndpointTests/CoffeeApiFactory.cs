using CoffeeLoverApi.Application.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CoffeeLoverApi.EndpointTests;

public sealed class CoffeeApiFactory : WebApplicationFactory<Program>
{
    private readonly IDateTimeProvider _clock;
    private readonly ICoffeeMachineRepository _repo;
    private readonly IWeatherService _weather;

    public CoffeeApiFactory(IDateTimeProvider clock, ICoffeeMachineRepository repo, IWeatherService weather)
    {
        _clock = clock;
        _repo = repo;
        _weather = weather;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Ensures Swagger is enabled if your Program.cs gates it by Development
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            // Replace production registrations with fakes
            services.RemoveAll<IDateTimeProvider>();
            services.RemoveAll<ICoffeeMachineRepository>();
            services.RemoveAll<IWeatherService>();

            services.AddSingleton(_clock);
            services.AddSingleton(_repo);
            services.AddSingleton(_weather);
        });
    }
}