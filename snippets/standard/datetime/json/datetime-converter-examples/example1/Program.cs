using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace DateTimeConverterExamples
{
    public class DateTimeConverterUsingDateTimeParse : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // typeToConvert will always be typeof(DateTime). The parameter is useful for
            // polymorphic cases and when using generics to get typeof(T) in a performant way.
            Debug.Assert(typeToConvert == typeof(DateTime));

            return DateTime.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }

    class Program
    {
        private static void ParseDateTimeWithDefaultOptions()
        {
            var _ = JsonSerializer.Deserialize<DateTime>(@"""04-10-2008 6:30 AM""");
            // Throws JsonException.
        }

        private static void FormatDateTimeWithDefaultOptions()
        {
            Console.WriteLine(JsonSerializer.Serialize(DateTime.Parse("04-10-2008 6:30 AM -4")));
            // "2008-04-10T06:30:00-04:00"
        }

        private static void ProcessDateTimeWithCustomConverter()
        {
            var options = new JsonSerializerOptions();
            options.Converters.Add(new DateTimeConverterUsingDateTimeParse());

            var testDateTimeStr = "04-10-2008 6:30 AM";
            var testDateTimeJson = @"""" + testDateTimeStr + @"""";

            var resultDateTime = JsonSerializer.Deserialize<DateTime>(testDateTimeJson, options);
            Console.WriteLine(resultDateTime);
            // 4/10/2008 6:30:00 AM

            string resultDateTimeJson = JsonSerializer.Serialize(DateTime.Parse(testDateTimeStr), options);
            Console.WriteLine(Regex.Unescape(resultDateTimeJson));
            // "4/10/2008 6:30:00 AM"
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
                // The JSON value could not be converted to System.DateTime. Path: $ | LineNumber: 0 | BytePositionInLine: 20.
            }

            // Formatting with default options prints according to extended ISO 8601 profile.
            FormatDateTimeWithDefaultOptions();

            // Using converters gives you control over the serializers parsing and formatting.
            ProcessDateTimeWithCustomConverter();
        }
    }
}

// The example displays the following output:
// The JSON value could not be converted to System.DateTime. Path: $ | LineNumber: 0 | BytePositionInLine: 20.
// "2008-04-10T06:30:00-04:00"
// 4/10/2008 6:30:00 AM
// "4/10/2008 6:30:00 AM"
