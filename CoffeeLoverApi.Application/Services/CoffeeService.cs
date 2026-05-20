using CoffeeLoverApi.Application.Abstractions;
using CoffeeLoverApi.Domain;

namespace CoffeeLoverApi.Application.Services
{

    public sealed class CoffeeService : ICoffeeService
    {
        private readonly IDateTimeProvider _clock;
        private readonly ICoffeeMachineRepository _repo;

        public CoffeeService(IDateTimeProvider clock, ICoffeeMachineRepository repo)
        {
            _clock = clock;
            _repo = repo;
        }

        public BrewResult BrewCoffee()
        {
            var now = _clock.Now();

            // 3) April 1st => always 418 (empty body)
            if (now.Month == 4 && now.Day == 1)
                return new BrewResult(BrewOutcome.Teapot);

            // 2) Every fifth call => 503 (empty body)
            var callNumber = _repo.IncrementAndGetBrewCount();
            if (callNumber % 5 == 0)
                return new BrewResult(BrewOutcome.OutOfCoffee);

            // 1) Otherwise => 200 with JSON
            var prepared = ToIso8601BasicOffset(now);
            var response = new BrewCoffeeResponse(
                Message: "Your piping hot coffee is ready",
                Prepared: prepared
            );

            return new BrewResult(BrewOutcome.Ok, response);
        }

        private static string ToIso8601BasicOffset(DateTimeOffset dto)
        {
            // Produces e.g. "2026-05-20T14:56:24+0800" (no colon in offset)
            var s = dto.ToString("yyyy-MM-dd'T'HH:mm:sszzz"); // +08:00
            return s[..^3] + s[^2..];                         // +0800
        }
    }
}
