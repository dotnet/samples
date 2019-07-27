using System;
using System.Buffers;
using System.Buffers.Text;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DateTimeConverterExamples
{
    public class DateTimeConverterExample2 : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }

            if (Utf8Parser.TryParse(reader.ValueSpan, out DateTime value, out _, 'R'))
            {
                reader.Read();
                return value;
            }

            throw new FormatException();
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            Span<byte> destination = new byte[29];

            bool result = Utf8Formatter.TryFormat(value, destination, out _, new StandardFormat('R'));
            Debug.Assert(result);

            writer.WriteStringValue(Encoding.UTF8.GetString(destination));
        }
    }

    class Program
    {
        private static void ParseDateTimeWithDefaultOptions_Example2()
        {
            var _ = JsonSerializer.Deserialize<DateTime>(@"""Thu, 25 Jul 2019 13:36:07 GMT"""); // Throws JsonException
        }

        private static void ProcessDateTimeWithCustomConverter_Example2()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new DateTimeConverterExample2());

            var testDateTimeStr = "Thu, 25 Jul 2019 13:36:07 GMT";
            var testDateTimeJson = @"""" + testDateTimeStr + @"""";

            var resultDateTime = JsonSerializer.Deserialize<DateTime>(testDateTimeJson, options);
            Console.WriteLine(resultDateTime); // 7/25/2019 1:36:07 PM

            Console.WriteLine(JsonSerializer.Serialize(DateTime.Parse(testDateTimeStr), options)); // "Thu, 25 Jul 2019 09:36:07 GMT"
        }

        static void Main(string[] args)
        {
            // Parsing non-compliant format as DateTime fails by default.
            try
            {
                ParseDateTimeWithDefaultOptions_Example2();
            }
            catch (JsonException e)
            {
                Console.WriteLine(e.Message); // The JSON value could not be converted to System.DateTime. Path: $ | LineNumber: 0 | BytePositionInLine: 31.
            }

            // Using converters gives you control over the serializers parsing and formatting.
            ProcessDateTimeWithCustomConverter_Example2();
        }
    }
}

// The example displays the following output:
// The JSON value could not be converted to System.DateTime.Path: $ | LineNumber: 0 | BytePositionInLine: 31.
// 7/25/2019 1:36:07 PM
// "Thu, 25 Jul 2019 09:36:07 GMT"
