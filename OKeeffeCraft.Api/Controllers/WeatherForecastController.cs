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

        [AllowAnonymous]
        [HttpGet(Name = "GetWeatherForecast")]
        [SwaggerOperation(Summary = "Gets the weather forecast for the next 5 days.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The weather forecast for the next 5 days.", typeof(ServiceResponse<IEnumerable<WeatherForecast>>))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The user is not authorized to access this resource.", typeof(ServiceResponse<IEnumerable<WeatherForecast>>))]
        public IActionResult Get()
        {
            return Ok(new ServiceResponse<string> { Data = null, Message = "Clear skys", Success = true });
        }

        [AllowAnonymous]
        [HttpGet("WeatherForecast")]
        [SwaggerOperation(Summary = "Pings the api confirming connection.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The api is connected.", typeof(ServiceResponse<string>))]
        public IActionResult Ping()
        {
            return Ok(new ServiceResponse<string> { Data = "API is connected.", Message = "Clear skys", Success = true });
        }

        [Authorize]
        [HttpGet("SecureWeatherForecast")]
        [SwaggerOperation(Summary = "Gets the weather forecast for the next 5 days.")]
        [SwaggerResponse(StatusCodes.Status200OK, "The weather forecast for the next 5 days.", typeof(string))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "The user is not authorized to access this resource.", typeof(ServiceResponse<IEnumerable<WeatherForecast>>))]
        public IActionResult SecureGet()
        {
            return BadRequest("Invalid request");
        }
    }
}
