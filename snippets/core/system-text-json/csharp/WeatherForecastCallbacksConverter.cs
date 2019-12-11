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
            // Place "before" code here (OnDeserializing), but note that there is no access here to the POCO instance.
            Console.WriteLine("OnDeserializing");

            // Don't pass in options when recursively calling Deserialize.
            WeatherForecast value = JsonSerializer.Deserialize<WeatherForecast>(ref reader);

            // Place "after" code here (OnDeserialized)
            Console.WriteLine("OnDeserialized");

            return value;
        }

        public override void Write(
            Utf8JsonWriter writer,
            WeatherForecast value, JsonSerializerOptions options)
        {
            // Place "before" code here (OnSerializing)
            Console.WriteLine("OnSerializing");

            // Don't pass in options when recursively calling Serialize.
            JsonSerializer.Serialize(writer, value);

            // Place "after" code here (OnSerialized)
            Console.WriteLine("OnSerialized");
        }
    }
}
