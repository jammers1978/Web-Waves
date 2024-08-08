using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Diagnostics;
using WebWaves.Server.AsyncRequests;

namespace Web_Waves.Server.Services.Weather
{
    public interface IWeatherService
    {
        IEnumerable<WeatherForecast> GetWeatherForecast(string asyncRequestId, CancellationToken cancellationToken = default);
        string GetWeatherSummary();
    }

    public class WeatherService : IWeatherService
    {
        private readonly WeatherServiceRepository _repository;

        public WeatherService(WeatherServiceRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public IEnumerable<WeatherForecast> GetWeatherForecast(string asyncRequestId, CancellationToken cancellationToken = default) 
        {
            //Fake long running process
            //instantiate as you need to use this in both case that there is cancellation and if not to clear token
            CancellationTokenManager cancellationTokenManager = new CancellationTokenManager();

            int count = 0;
            int lengthOfFakeLongRunningProcessSeconds = 9;
            while (count < lengthOfFakeLongRunningProcessSeconds)
            {
                Debug.WriteLine("Count: " + (++count) + " seconds");
                Thread.Sleep(1000);
                if (cancellationToken.IsCancellationRequested)
                {
                    Debug.WriteLine($"Task cancelled via cancellation token at : {count} seconds");
                    cancellationTokenManager.ClearToken(asyncRequestId);
                    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                    {
                        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                        TemperatureC = (int)99999,
                        Summary = "FETCH CANCELLED THIS IS INNACCURATE"
                    })
                    .ToArray();
                }
            }
            
            Debug.WriteLine($"Count reached {count} seconds. Stopping loop.");

            cancellationTokenManager.ClearToken(asyncRequestId);

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = this.GetWeatherSummary()
            })
            .ToArray();
        }

        public string GetWeatherSummary()
        {
            return _repository.GetWeatherSummary();
        }
    }
}