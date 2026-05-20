using CoffeeLoverApi.Application.Abstractions;

namespace CoffeeLoverApi.UnitTests.Fakes
{
    public class FakeCoffeeMachineRepository : ICoffeeMachineRepository
    {
        private long _count = 0;

        public long IncrementAndGetBrewCount()
        {
            _count++;
            return _count;
        }
    }
}
