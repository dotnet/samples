using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemTextJsonSamples
{
    public class TemperatureConverter : JsonConverter<Temperature>
    {
        public override Temperature Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options) =>
                Temperature.Parse(reader.GetString());

        public override void Write(
            Utf8JsonWriter writer,
            Temperature value,
            JsonSerializerOptions options) =>
                writer.WriteStringValue(value.ToString());
    }
}
