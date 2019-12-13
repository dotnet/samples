using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemTextJsonSamples
{
    public class DeserializeNullToNonnullableType
    {
        public static void Run()
        {
            string jsonString;
            var wf = WeatherForecastFactories.CreateWeatherForecast();

            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.WriteIndented = true;
            serializeOptions.Converters.Add(new DateTimeOffsetNullHandlingConverter());
            jsonString = JsonSerializer.Serialize(wf, serializeOptions);
            Console.WriteLine($"JSON with valid Date:\n{jsonString}\n");

            var deserializeOptions = new JsonSerializerOptions();
            deserializeOptions.Converters.Add(new DateTimeOffsetNullHandlingConverter());
            wf = JsonSerializer.Deserialize<WeatherForecast>(jsonString, deserializeOptions);
            wf.DisplayPropertyValues();

            jsonString = @"{""Date"": null,""TemperatureCelsius"": 25,""Summary"":""Hot""}";
            Console.WriteLine($"JSON with null Date:\n{jsonString}\n");

            // The missing-date JSON deserializes with error if the converter isn't used.
            try
            {
                wf = JsonSerializer.Deserialize<WeatherForecast>(jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception thrown: {ex.Message}\n");
            }

            Console.WriteLine("Deserialize with converter");
            deserializeOptions = new JsonSerializerOptions();
            deserializeOptions.Converters.Add(new DateTimeOffsetNullHandlingConverter());
            wf = JsonSerializer.Deserialize<WeatherForecast>(jsonString, deserializeOptions);
            wf.DisplayPropertyValues();
        }
    }
   
}
