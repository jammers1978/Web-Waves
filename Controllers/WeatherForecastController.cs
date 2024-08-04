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

namespace WebWaves.Server.Controllers
{
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
            CancellationTokenSourceCacheData ctsCacheData = cancellationTokenManager.GetCancellationTokenSourceForSession("cancellationTest");
            CancellationToken cancellationToken = ctsCacheData.CancellationTokenSource.Token;

            return _weatherService.GetWeatherForecast(cancellationToken);
        }

        [HttpPost("CancelWeatherForecastFetch", Name = "CancelWeatherForecastFetch")]
        public IActionResult CancelWeatherForecastFetch()
        {
            CancellationTokenManager cancellationTokenManager = new CancellationTokenManager();
            CancellationTokenSourceCacheData ctsCacheData = cancellationTokenManager.GetCancellationTokenSourceForSession("cancellationTest");
            CancellationTokenSource cancellationTokenSource = ctsCacheData.CancellationTokenSource;

            cancellationTokenSource.Cancel();

            return Ok("Weather forecast fetch cancellation initiated.");
        }
    }
}
