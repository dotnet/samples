using System;
using System.Text.Json;

namespace SystemTextJsonSamples
{
    class RoundtripPropertyNamesByAttribute
    {
        public static void Run()
        {
            string jsonString;
            var weatherForecast = WeatherForecastFactories.CreateWeatherForecastWithPropertyNameAttribute();
            weatherForecast.DisplayPropertyValues();

            // <SnippetSerialize>
            var serializeOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            jsonString = JsonSerializer.Serialize(weatherForecast, serializeOptions);
            // </SnippetSerialize>
            Console.WriteLine($"JSON output:\n{jsonString}\n");

            // <SnippetDeserialize>
            weatherForecast = JsonSerializer.Deserialize<WeatherForecastWithPropertyNameAttribute>(jsonString);
            weatherForecast.DisplayPropertyValues();
            // </SnippetDeserialize>
        }
    }
}