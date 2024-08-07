using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Web_Waves.Server.Services.Weather;
using WebWaves.Server.AsyncRequests;
using static WebWaves.Server.AsyncRequests.CancellationTokenManager;
using Microsoft.AspNetCore.Authorization;
using Web_Waves;
using System.Security.Claims;

namespace WebWaves.Server.Controllers
{
    [Authorize] 
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly WeatherService _weatherService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, WeatherService weatherService)
        {
            _logger = logger;
            _weatherService = weatherService ?? throw new ArgumentNullException(nameof(weatherService));
        }

        [HttpGet("GetWeatherForecast", Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {


            CancellationTokenManager cancellationTokenManager = new CancellationTokenManager();
            //It's a fetch of data so start again
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier)?? "";

            CancellationTokenSourceCacheData ctsCacheData = cancellationTokenManager.GetCancellationTokenSourceForSession($"weather{userId}");

            CancellationToken cancellationToken = ctsCacheData.CancellationTokenSource.Token;

            return _weatherService.GetWeatherForecast(cancellationToken);
        }

        [HttpPost("CancelWeatherForecastFetch", Name = "CancelWeatherForecastFetch")]
        public IActionResult CancelWeatherForecastFetch()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

            CancellationTokenManager cancellationTokenManager = new CancellationTokenManager();
            CancellationTokenSourceCacheData ctsCacheData = cancellationTokenManager.GetCancellationTokenSourceForSession($"weather{userId}");
            if (ctsCacheData != null)
            {
                CancellationTokenSource cancellationTokenSource = ctsCacheData.CancellationTokenSource;
                cancellationTokenSource.Cancel(); 
            }
            else 
            {
                return Ok("Weather forecast fetch already cancelled.");
            }
            
            return Ok("Weather forecast fetch cancellation initiated.");
        }
    }
}
