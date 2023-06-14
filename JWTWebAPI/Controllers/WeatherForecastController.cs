using JWTWebAPI.FilterAttribut;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JWTWebAPI.Controllers
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

        [HttpGet(Name = "GetWeatherForecast")]
        [ServiceFilter(typeof(CheckCookiesFilterAttribut))]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("GetMe"), Authorize]
        [ServiceFilter(typeof(CheckCookiesFilterAttribut))]
        public ActionResult GetMe()
        {
            // Get info of Token in request Context 
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            var userClaim = identity!.Claims;

            var info = GetInfo();
            return Ok($"{userClaim.FirstOrDefault(x => x.Type == ClaimTypes.Role)!.Value} :  {userClaim.FirstOrDefault(y => y.Type == ClaimTypes.NameIdentifier)!.Value} , {info[0]} {info[1]}");
        }

        // or you can create an metode to get info from request context
        private List<string> GetInfo()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userClaim = identity!.Claims;
            return new List<string> { userClaim.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)!.Value.ToString(), userClaim.FirstOrDefault(y => y.Type == ClaimTypes.Role)!.Value.ToString() };

        }
    }
}