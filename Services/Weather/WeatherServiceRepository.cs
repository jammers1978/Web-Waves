namespace Web_Waves.Server.Services.Weather
{
    public class WeatherServiceRepository
    {
        private static readonly string[] Summaries = new[]
       {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public string GetWeatherSummary() 
        {
            return Summaries[Random.Shared.Next(Summaries.Length)];
        }
    }
}
