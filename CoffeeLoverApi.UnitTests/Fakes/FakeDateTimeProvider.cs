using CoffeeLoverApi.Application.Abstractions;
using System;

namespace CoffeeLoverApi.UnitTests.Fakes
{
    public class FakeDateTimeProvider : IDateTimeProvider
    {
        private readonly DateTimeOffset _fixedDate;

        public FakeDateTimeProvider(DateTimeOffset fixedDate)
        {
            _fixedDate = fixedDate;
        }

        public DateTimeOffset Now() => _fixedDate;
    }

}
