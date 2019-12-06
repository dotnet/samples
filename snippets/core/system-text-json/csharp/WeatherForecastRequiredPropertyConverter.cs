using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemTextJsonSamples
{
    public class WeatherForecastRequiredPropertyConverter : JsonConverter<WeatherForecast>
    {
        public override WeatherForecast Read(
            ref Utf8JsonReader reader,
            Type type,
            JsonSerializerOptions options)
        {
            WeatherForecast value = JsonSerializer.Deserialize<WeatherForecast>(ref reader); // note: "options" not passed in

            // Check for required fields set by values in JSON
            if (value.Date == default)
            {
                throw new JsonException("Required property not received in the JSON");
            };
            return value;
        }

        public override void Write(
            Utf8JsonWriter writer,
            WeatherForecast value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value); // note: "options" not passed in
        }
    }
}
