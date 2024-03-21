using Microsoft.AspNetCore.Mvc;
using OKeeffeCraft.Authorization;
using OKeeffeCraft.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace OKeeffeCraft.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet(Name = "GetWeatherForecast")]
        [SwaggerOperation(Summary = "Gets the weather forecast for the next 5 days.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The weather forecast for the next 5 days.", typeof(ServiceResponse<IEnumerable<WeatherForecast>>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The user is not authorized to access this resource.", typeof(ServiceResponse<IEnumerable<WeatherForecast>>))]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [AllowAnonymous]
        [HttpGet("Ping")]
        [SwaggerOperation(Summary = "Pings the api confirming connection.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The api is connected.", typeof(ServiceResponse<string>))]
        public IActionResult Ping()
        {
            return Ok(new ServiceResponse<string> { Data = null, Message = "Hello World", Success = true });
        }
    }
}
