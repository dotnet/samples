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
            jsonString = @"{""Date"": null,""TemperatureCelsius"": 25,""Summary"":""Hot""}";
            // <SnippetDeserialize>
            var deserializeOptions = new JsonSerializerOptions();
            deserializeOptions.Converters.Add(new WeatherForecastConverter());
            wf = JsonSerializer.Deserialize<WeatherForecast>(jsonString, deserializeOptions);
            // </SnippetDeserialize>
            wf.DisplayPropertyValues();
        }
    }
   
}
