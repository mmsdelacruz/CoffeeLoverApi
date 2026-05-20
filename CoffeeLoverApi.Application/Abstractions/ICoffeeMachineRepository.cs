namespace CoffeeLoverApi.Application.Abstractions
{
    public interface ICoffeeMachineRepository
    {
        /// <summary>
        /// Increments brew request count and returns the new total.
        /// </summary>
        long IncrementAndGetBrewCount();
    }
}
