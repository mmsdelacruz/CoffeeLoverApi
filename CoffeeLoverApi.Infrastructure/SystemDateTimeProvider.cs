using CoffeeLoverApi.Application.Abstractions;

namespace CoffeeLoverApi.Infrastructure
{

    public sealed class SystemDateTimeProvider : IDateTimeProvider
    {
        public DateTimeOffset Now() => DateTimeOffset.Now;
    }
}
