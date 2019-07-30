using System;
using System.Buffers;
using System.Buffers.Text;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DateTimeConverterExamples
{
    public class DateTimeConverterUsingDateTimeParseAsFallback : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }

            if (!reader.TryGetDateTime(out DateTime value))
            {
                value = DateTime.Parse(reader.GetString());
            }

            return value;
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            // Converters allow support for custom formats.
            writer.WriteStringValue(value.ToString("dd/MM/yyy"));
        }
    }

    class Program
    {
        private static void ParseDateTimeWithDefaultOptions()
        {
            var _ = JsonSerializer.Deserialize<DateTime>(@"""2019-07-16 16:45:27.4937872+00:00""");
            // Throws JsonException
        }

        private static void ProcessDateTimeWithCustomConverter()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new DateTimeConverterUsingDateTimeParseAsFallback());

            var testDateTimeStr = "2019-07-16 16:45:27.4937872+00:00";
            var testDateTimeJson = @"""" + testDateTimeStr + @"""";

            var resultDateTime = JsonSerializer.Deserialize<DateTime>(testDateTimeJson, options);
            Console.WriteLine(resultDateTime);
            // 7/16/2019 4:45:27 PM

            Console.WriteLine(JsonSerializer.Serialize(DateTime.Parse(testDateTimeStr), options));
            // "16.07.2019"
        }

        static void Main(string[] args)
        {
            // Parsing non-compliant format as DateTime fails by default.
            try
            {
                ParseDateTimeWithDefaultOptions();
            }
            catch (JsonException e)
            {
                Console.WriteLine(e.Message);
                // The JSON value could not be converted to System.DateTime. Path: $ | LineNumber: 0 | BytePositionInLine: 31.
            }

            // Using converters gives you control over the serializers parsing and formatting.
            ProcessDateTimeWithCustomConverter();
        }
    }
}

// The example displays the following output:
// The JSON value could not be converted to System.DateTime.Path: $ | LineNumber: 0 | BytePositionInLine: 31.
// 7/25/2019 1:36:07 PM
// "Thu, 25 Jul 2019 09:36:07 GMT"
