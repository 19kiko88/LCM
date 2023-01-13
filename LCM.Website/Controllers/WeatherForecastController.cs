using LCM.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LCM.Website.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly CAEDB01Context _context;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, CAEDB01Context context)
        {
            _logger = logger;
            _context= context;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<Object> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("QQ")]
        public IEnumerable<Object> GetTestData()
        {
            return _context.XxPor0001_Resell.ToList();
        }
    }
}