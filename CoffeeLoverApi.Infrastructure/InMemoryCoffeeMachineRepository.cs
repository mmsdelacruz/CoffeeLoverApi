using CoffeeLoverApi.Application.Abstractions;

namespace CoffeeLoverApi.Infrastructure
{

    public sealed class InMemoryCoffeeMachineRepository : ICoffeeMachineRepository
    {
        private long _brewCount = 0;

        public long IncrementAndGetBrewCount()
            => Interlocked.Increment(ref _brewCount);
    }
}
