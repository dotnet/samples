using System;
using System.Text.Json;

namespace SystemTextJsonSamples
{
    public class SerializePolymorphic
    {
        public static void Run()
        {
            string jsonString;
            var weatherForecastDerived = WeatherForecastFactories.CreateWeatherForecastDerived();
            WeatherForecast weatherForecast = weatherForecastDerived;
            weatherForecast.DisplayPropertyValues();

            Console.WriteLine("Base class generic type - derived class properties omitted");
            // <SnippetSerializeDefault>
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            jsonString = JsonSerializer.Serialize<WeatherForecast>(weatherForecast, options);
            // </SnippetSerializeDefault>

            Console.WriteLine($"JSON output:\n{jsonString}\n");

            Console.WriteLine("Object generic type parameter - derived class properties included");
            // <SnippetSerializeObject>
            options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            jsonString = JsonSerializer.Serialize<object>(weatherForecast, options);
            // </SnippetSerializeObject>
            Console.WriteLine($"JSON output:\n{jsonString}\n");


            Console.WriteLine("GetType() type parameter - derived class properties included");
            // <SnippetSerializeGetType>
            options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            jsonString = JsonSerializer.Serialize(weatherForecast, weatherForecast.GetType(), options);
            // </SnippetSerializeGetType>
            Console.WriteLine($"JSON output:\n{jsonString}\n");

            WeatherForecastWithPrevious weatherForecastWithPrevious = WeatherForecastFactories.CreateWeatherForecastWithPrevious();
            WeatherForecastWithPreviousAsObject weatherForecastWithPreviousAsObject = WeatherForecastFactories.CreateWeatherForecastWithPreviousAsObject();

            Console.WriteLine("GetType() type parameter - second level derived class properties included only for object properties");
            // <SnippetSerializeSecondLevel>
            options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            jsonString = JsonSerializer.Serialize(weatherForecastWithPreviousAsObject, options);
            // </SnippetSerializeSecondLevel>
            Console.WriteLine($"JSON output with WindSpeed:\n{jsonString}\n");
            jsonString = JsonSerializer.Serialize(
                weatherForecastWithPrevious,
                options);
            Console.WriteLine($"JSON output without WindSpeed:\n{jsonString}\n");
        }
    }
}
