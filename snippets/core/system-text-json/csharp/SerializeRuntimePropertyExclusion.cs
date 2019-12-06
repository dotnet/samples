using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemTextJsonSamples
{
    public class SerializeRuntimePropertyExclusion
    {
        public static void Run()
        {
            string jsonString;
            var wf = WeatherForecastFactories.CreateWeatherForecastWithTemperatureAttribute();
            wf.DisplayPropertyValues();

            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new OmitOutOfRangeTemperatureConverter());
            serializeOptions.WriteIndented = true;
            jsonString = JsonSerializer.Serialize(wf, serializeOptions);
            Console.WriteLine($"JSON output:\n{jsonString}\n");

            wf.TemperatureCelsius = 91;
            wf.DisplayPropertyValues();
            serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new OmitOutOfRangeTemperatureConverter());
            serializeOptions.WriteIndented = true;
            jsonString = JsonSerializer.Serialize(wf, serializeOptions);
            Console.WriteLine($"JSON output:\n{jsonString}\n");

            var deserializeOptions = new JsonSerializerOptions();
            deserializeOptions.Converters.Add(new OmitOutOfRangeTemperatureConverter());
            wf = JsonSerializer.Deserialize<WeatherForecastWithTemperatureAttribute>(jsonString, deserializeOptions);
            wf.DisplayPropertyValues();
        }
    }
   
}
