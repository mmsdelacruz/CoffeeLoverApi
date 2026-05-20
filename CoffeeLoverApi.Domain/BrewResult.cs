namespace CoffeeLoverApi.Domain
{
    public sealed record BrewResult(
        BrewOutcome Outcome,
        BrewCoffeeResponse? Response = null
    );
}
