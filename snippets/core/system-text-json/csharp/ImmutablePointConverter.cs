using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemTextJsonSamples
{
    public class ImmutablePointConverter : JsonConverter<ImmutablePoint>

    {
        private const string XName = "X";
        private const string YName = "Y";

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
            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            string propertyName = reader.GetString();
            if (propertyName == XName)
            {
                x = ReadProperty(ref reader, options);
                xSet = true;
            }
            else if (propertyName == YName)
            {
                y = ReadProperty(ref reader, options);
                ySet = true;
            }
            else
            {
                throw new JsonException();
            }

            // Get the second property.
            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            propertyName = reader.GetString();
            if (propertyName == XName)
            {
                x = ReadProperty(ref reader, options);
                xSet = true;
            }
            else if (propertyName == YName)
            {
                y = ReadProperty(ref reader, options);
                ySet = true;
            }
            else
            {
                throw new JsonException();
            }

            if (!xSet || !ySet)
            {
                throw new JsonException();
            }

            reader.Read();

            if (reader.TokenType != JsonTokenType.EndObject)
            {
                throw new JsonException();
            }

            return new ImmutablePoint(x, y);
        }

        public int ReadProperty(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            if (options?.GetConverter(typeof(int)) is JsonConverter<int> intConverter)
            {
                reader.Read();
                return intConverter.Read(ref reader, typeof(int), options);
            }
            else
            {
                throw new JsonException();
            }
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
