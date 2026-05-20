using CoffeeLoverApi.Application.Abstractions;

namespace CoffeeLoverApi.EndpointTests.Fakes
{
    public sealed class FixedDateTimeProvider : IDateTimeProvider
    {
        private readonly DateTimeOffset _value;

        public FixedDateTimeProvider(DateTimeOffset value) => _value = value;

        public DateTimeOffset Now() => _value;
    }
}
