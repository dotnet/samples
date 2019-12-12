using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemTextJsonSamples
{
    public class DeserializeRequiredProperty
    {
        public static void Run()
        {
            string jsonString;
            var wf = WeatherForecastFactories.CreateWeatherForecast();

            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.WriteIndented = true;
            serializeOptions.Converters.Add(new WeatherForecastRequiredPropertyConverter());
            jsonString = JsonSerializer.Serialize(wf, serializeOptions);
            Console.WriteLine($"JSON with Date:\n{jsonString}\n");

            var deserializeOptions = new JsonSerializerOptions();
            deserializeOptions.Converters.Add(new WeatherForecastRequiredPropertyConverter());
            wf = JsonSerializer.Deserialize<WeatherForecast>(jsonString, deserializeOptions);
            wf.DisplayPropertyValues();

            jsonString = @"{""TemperatureCelsius"": 25,""Summary"":""Hot""}";
            Console.WriteLine($"JSON without Date:\n{jsonString}\n");

            // The missing-date JSON deserializes without error if the converter isn't used.
            Console.WriteLine("Deserialize without converter");
            wf = JsonSerializer.Deserialize<WeatherForecast>(jsonString);
            wf.DisplayPropertyValues();

            Console.WriteLine("Deserialize with converter");
            try
            {
                deserializeOptions = new JsonSerializerOptions();
                deserializeOptions.Converters.Add(new WeatherForecastRequiredPropertyConverter());
                wf = JsonSerializer.Deserialize<WeatherForecast>(jsonString, deserializeOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception thrown: {ex.Message}\n");
            }
            // wf object is unchanged if exception is thrown.
            wf.DisplayPropertyValues();
        }
    }
   
}
