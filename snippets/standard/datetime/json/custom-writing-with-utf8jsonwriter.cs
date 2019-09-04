using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;

public class Example
{
    public static void Main(string[] args)
    {
        var options = new JsonWriterOptions
        {
            Indented = true
        };

        using (var stream = new MemoryStream())
        {
            using (var writer = new Utf8JsonWriter(stream, options))
            {
                DateTime utcNow = DateTime.UtcNow;
                string dateStr = utcNow.ToString("F", CultureInfo.InvariantCulture);

                writer.WriteStartObject();
                writer.WritePropertyName("dateFormats");
                writer.WriteStartArray();
                writer.WriteStringValue(utcNow);
                writer.WriteStringValue(dateStr);
                writer.WriteEndArray();
                writer.WriteNumber("temp", 42);
                writer.WriteEndObject();
            }

            string json = Encoding.UTF8.GetString(stream.ToArray());
            Console.WriteLine(json);
        }
    }
}

// The example displays output similar to the following:
// {
//     "dateFormats": [
//         "2019-09-04T17:38:04.6940145Z",
//         "Wednesday, 04 September 2019 17:38:04"
//     ],
//     "temp": 42
// }
