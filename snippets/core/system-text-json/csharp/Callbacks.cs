using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemTextJsonSamples
{
    public class Callbacks
    {
        public static void Run()
        {
            string jsonString;
            var wf = WeatherForecastFactories.CreateWeatherForecast();
            wf.DisplayPropertyValues();

            // <SnippetRegister>
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new WeatherForecastConverter());
            // </SnippetRegister>
            serializeOptions.WriteIndented = true;
            jsonString = JsonSerializer.Serialize(wf, serializeOptions);
            Console.WriteLine($"JSON output:\n{jsonString}\n");
            // <SnippetDeserialize>
            jsonString = @"{""Date"": null,""TemperatureCelsius"": 25,""Summary"":""Hot""}";
            var deserializeOptions = new JsonSerializerOptions();
            deserializeOptions.Converters.Add(new WeatherForecastConverter());
            wf = JsonSerializer.Deserialize<WeatherForecast>(jsonString, deserializeOptions);
            wf.DisplayPropertyValues();

            jsonString = @"{""TemperatureCelsius"": 25,""Summary"":""Hot""}";
            deserializeOptions = new JsonSerializerOptions();
            deserializeOptions.Converters.Add(new WeatherForecastConverter());
            try
            {
                wf = JsonSerializer.Deserialize<WeatherForecast>(jsonString, deserializeOptions);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"{ex.Message} Path={ex.Path}");
            }
            // </SnippetDeserialize>
            wf.DisplayPropertyValues();
        }
    }
   
}
