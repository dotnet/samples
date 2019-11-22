using System;
using System.Text.Json;

namespace SystemTextJsonSamples
{
    public class ConvertLongToString
    {
        public static void Run()
        {
            string jsonString;

            // Serialize to create input JSON
            var weatherForecast = WeatherForecastFactories.CreateWeatherForecastWithLong();
            var serializeOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            jsonString = JsonSerializer.Serialize(weatherForecast, serializeOptions);
            Console.WriteLine($"JSON output:\n{jsonString}\n");

            weatherForecast = JsonSerializer.Deserialize<WeatherForecastWithLong>(jsonString);
            weatherForecast.DisplayPropertyValues();
        }
    }
}
