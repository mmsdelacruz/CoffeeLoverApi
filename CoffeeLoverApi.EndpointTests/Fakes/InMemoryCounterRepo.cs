using CoffeeLoverApi.Application.Abstractions;

namespace CoffeeLoverApi.EndpointTests.Fakes
{
    public sealed class InMemoryCounterRepo : ICoffeeMachineRepository
    {
        private long _count;

        public long IncrementAndGetBrewCount() => Interlocked.Increment(ref _count);
    }
}
