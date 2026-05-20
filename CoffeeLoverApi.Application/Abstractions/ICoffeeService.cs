using CoffeeLoverApi.Domain;

namespace CoffeeLoverApi.Application.Abstractions
{

    public interface ICoffeeService
    {
        BrewResult BrewCoffee();
    }

}
