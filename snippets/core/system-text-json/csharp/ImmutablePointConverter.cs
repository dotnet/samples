using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemTextJsonSamples
{
    public class ImmutablePointConverter : JsonConverter<ImmutablePoint>
    {
        private readonly JsonEncodedText XName = JsonEncodedText.Encode("X");
        private readonly JsonEncodedText YName = JsonEncodedText.Encode("Y");

        private readonly JsonConverter<int> _intConverter;

        public ImmutablePointConverter(JsonSerializerOptions options)
        {
            if (options?.GetConverter(typeof(int)) is JsonConverter<int> intConverter)
            {
                _intConverter = intConverter;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public override ImmutablePoint Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            };

            int x = default;
            bool xSet = false;

            int y = default;
            bool ySet = false;

            // Get the first property.
            ReadXorY(ref reader, options, ref x, ref xSet, ref y, ref ySet);

            // Get the second property.
            ReadXorY(ref reader, options, ref x, ref xSet, ref y, ref ySet);

            reader.Read();

            if (reader.TokenType != JsonTokenType.EndObject)
            {
                throw new JsonException();
            }

            return new ImmutablePoint(x, y);
        }

        private void ReadXorY(ref Utf8JsonReader reader, JsonSerializerOptions options, ref int x, ref bool xSet, ref int y, ref bool ySet)
        {
            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            if (reader.ValueTextEquals(XName.EncodedUtf8Bytes))
            {
                x = ReadProperty(ref reader, options);
                xSet = true;
            }
            else if (reader.ValueTextEquals(YName.EncodedUtf8Bytes))
            {
                y = ReadProperty(ref reader, options);
                ySet = true;
            }
            else
            {
                throw new JsonException();
            }
        }

        private int ReadProperty(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            Debug.Assert(reader.TokenType == JsonTokenType.PropertyName);

            reader.Read();
            return _intConverter.Read(ref reader, typeof(int), options);
        }

        public override void Write(
            Utf8JsonWriter writer,
            ImmutablePoint value,
            JsonSerializerOptions options)
        {
            // Don't pass in options when recursively calling Serialize.
            JsonSerializer.Serialize(writer, value);
        }
    }
}
