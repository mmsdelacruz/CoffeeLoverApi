using CoffeeLoverApi.Domain;

namespace CoffeeLoverApi.Application.Abstractions
{
    public interface ICoffeeService
    {
        Task<BrewResult> BrewCoffeeAsync(CancellationToken ct = default);
    }
}
