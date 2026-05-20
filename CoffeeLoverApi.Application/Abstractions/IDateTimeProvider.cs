namespace CoffeeLoverApi.Application.Abstractions
{
    public interface IDateTimeProvider
    {
        DateTimeOffset Now();
    }
}
