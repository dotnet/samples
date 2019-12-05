using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemTextJsonSamples
{
    public class WeatherForecastCallbacksConverter : JsonConverter<WeatherForecast>
    {
        public override WeatherForecast Read(
            ref Utf8JsonReader reader,
            Type type,
            JsonSerializerOptions options)
        {
            // "before" code needs to go into the POCO constructor; code here doesn't have the POCO instance.

            WeatherForecast value = JsonSerializer.Deserialize<WeatherForecast>(ref reader); // note: "options" not passed in

            // Place "after" code here (e.g. OnDeserialized)
            Console.WriteLine("OnDeserialized");

            return value;
        }

        public override void Write(
            Utf8JsonWriter writer,
            WeatherForecast value, JsonSerializerOptions options)
        {
            // Place "before" code here (e.g. OnSerializing)
            Console.WriteLine("OnSerializing");

            JsonSerializer.Serialize(writer, value); // note: "options" not passed in

            // Place "after" code here (e.g. OnSerialized)
            Console.WriteLine("OnSerialized");
        }
    }
}
