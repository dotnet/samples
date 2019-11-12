using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SystemTextJsonSamples
{
    class RoundtripToFile
    {
        public static void Run()
        {
            string fileName = "WeatherForecast.json";
            var weatherForecast = WeatherForecastFactories.CreateWeatherForecast();
            weatherForecast.DisplayPropertyValues();

            // <SnippetSerialize>
            string jsonString = JsonSerializer.Serialize(weatherForecast);
            File.WriteAllText(fileName, jsonString);
            // </SnippetSerialize>
            Console.WriteLine($"The result is in {fileName}\n");

            // <SnippetDeserialize>
            jsonString = File.ReadAllText(fileName);
            weatherForecast = JsonSerializer.Deserialize<WeatherForecast>(jsonString);
            // </SnippetDeserialize>
            weatherForecast.DisplayPropertyValues();
        }
    }
}
