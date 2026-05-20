using CoffeeLoverApi.Application.Services;
using CoffeeLoverApi.Domain;
using CoffeeLoverApi.UnitTests.Fakes;
using System;
using Xunit;

namespace CoffeeLoverApi.UnitTests
{

    public class CoffeeServiceTests
    {
        [Fact]
        public void FirstCall_ShouldReturnCoffee()
        {
            // Arrange
            var clock = new FakeDateTimeProvider(
                new DateTimeOffset(2026, 5, 20, 10, 0, 0, TimeSpan.FromHours(8))
            );

            var repo = new FakeCoffeeMachineRepository();
            var service = new CoffeeService(clock, repo);

            // Act
            var result = service.BrewCoffee();

            // Assert
            Assert.Equal(BrewOutcome.Ok, result.Outcome);
            Assert.NotNull(result.Response);
            Assert.Equal("Your piping hot coffee is ready", result.Response!.Message);
        }

        [Fact]
        public void FifthCall_ShouldReturnOutOfCoffee()
        {
            // Arrange
            var clock = new FakeDateTimeProvider(DateTimeOffset.Now);
            var repo = new FakeCoffeeMachineRepository();
            var service = new CoffeeService(clock, repo);

            BrewResult? result = null;

            // Act - call 5 times
            for (int i = 0; i < 5; i++)
            {
                result = service.BrewCoffee();
            }

            // Assert
            Assert.NotNull(result);
            Assert.Equal(BrewOutcome.OutOfCoffee, result!.Outcome);
            Assert.Null(result.Response);
        }

        [Fact]
        public void AprilFirst_ShouldAlwaysReturnTeapot()
        {
            // Arrange (April 1)
            var clock = new FakeDateTimeProvider(
                new DateTimeOffset(2026, 4, 1, 10, 0, 0, TimeSpan.Zero)
            );

            var repo = new FakeCoffeeMachineRepository();
            var service = new CoffeeService(clock, repo);

            // Act (call multiple times)
            var result1 = service.BrewCoffee();
            var result2 = service.BrewCoffee();

            // Assert
            Assert.Equal(BrewOutcome.Teapot, result1.Outcome);
            Assert.Equal(BrewOutcome.Teapot, result2.Outcome);
        }

        [Fact]
        public void PreparedDate_ShouldBeIso8601WithoutColonOffset()
        {
            // Arrange
            var testDate = new DateTimeOffset(
                2026, 5, 20, 10, 30, 0, TimeSpan.FromHours(8)
            );

            var clock = new FakeDateTimeProvider(testDate);
            var repo = new FakeCoffeeMachineRepository();
            var service = new CoffeeService(clock, repo);

            // Act
            var result = service.BrewCoffee();

            // Assert
            Assert.Equal(BrewOutcome.Ok, result.Outcome);
            var prepared = result.Response!.Prepared;

            // Example expected: 2026-05-20T10:30:00+0800
            Assert.Contains("2026-05-20T10:30:00", prepared);
            Assert.EndsWith("+0800", prepared); // no colon
        }
    }
}
