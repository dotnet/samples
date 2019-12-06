using System;
using System.Buffers;
using System.Buffers.Text;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemTextJsonSamples
{
    public class OmitOutOfRangeTemperatureConverter : JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
        {
            return reader.GetInt32();
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            if (value > -90 && value < 90)
            {
                writer.WriteNumberValue(value);
            }
        }
    }
}
