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

    public CoffeeApiFactory(IDateTimeProvider clock, ICoffeeMachineRepository repo)
    {
        _clock = clock;
        _repo = repo;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Ensure Swagger is enabled in tests (since Program.cs uses IsDevelopment gating)
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            // Replace production registrations with test doubles
            services.RemoveAll<IDateTimeProvider>();
            services.RemoveAll<ICoffeeMachineRepository>();

            services.AddSingleton(_clock);
            services.AddSingleton(_repo);
        });
    }
}