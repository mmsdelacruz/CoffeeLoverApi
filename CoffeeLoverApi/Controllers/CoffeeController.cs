using CoffeeLoverApi.Application.Abstractions;
using CoffeeLoverApi.Domain;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeLoverApi.Api.Controllers;

[ApiController]
public sealed class CoffeeController : ControllerBase
{
    private readonly ICoffeeService _coffeeService;

    public CoffeeController(ICoffeeService coffeeService) => _coffeeService = coffeeService;

    [HttpGet("/brew-coffee")]
    public async Task<IActionResult> BrewCoffee(CancellationToken ct)
    {
        var result = await _coffeeService.BrewCoffeeAsync(ct);

        return result.Outcome switch
        {
            BrewOutcome.Teapot => EmptyStatus(StatusCodes.Status418ImATeapot),
            BrewOutcome.OutOfCoffee => EmptyStatus(StatusCodes.Status503ServiceUnavailable),
            BrewOutcome.Ok => Ok(result.Response),
            _ => EmptyStatus(StatusCodes.Status500InternalServerError)
        };
    }

    private IActionResult EmptyStatus(int statusCode)
    {
        HttpContext.Response.StatusCode = statusCode;
        return new EmptyResult();
    }
}