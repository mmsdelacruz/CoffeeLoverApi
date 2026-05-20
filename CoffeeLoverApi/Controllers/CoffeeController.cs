using CoffeeLoverApi.Application.Abstractions;
using CoffeeLoverApi.Domain;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeLoverApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CoffeeController : ControllerBase
    {

        private readonly ICoffeeService _coffeeService;

        public CoffeeController(ICoffeeService coffeeService)
            => _coffeeService = coffeeService;

        [HttpGet("/brew-coffee")]
        public IActionResult BrewCoffee()
        {
            var result = _coffeeService.BrewCoffee();

            switch (result.Outcome)
            {
                case BrewOutcome.Teapot:
                    HttpContext.Response.StatusCode = StatusCodes.Status418ImATeapot;
                    return new EmptyResult();

                case BrewOutcome.OutOfCoffee:
                    HttpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                    return new EmptyResult();

                case BrewOutcome.Ok:
                    return Ok(result.Response);

                default:
                    HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    return new EmptyResult();
            }
        }
    }
}
