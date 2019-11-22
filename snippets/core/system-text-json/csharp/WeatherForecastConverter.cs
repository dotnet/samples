using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemTextJsonSamples
{
    public class WeatherForecastConverter : JsonConverter<WeatherForecast>
    {
        public override WeatherForecast Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Location for OnDeserializing "callback" code.
            Console.WriteLine("OnDeserializing");
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            var wf = new WeatherForecast();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    // Location for OnDeserialized "callback" code.
                    Console.WriteLine("OnDeserialized");
                    // Check for required fields set by values in JSON
                    if (wf.Date == default(DateTimeOffset))
                    {
                        throw new JsonException("Required property not received in the JSON");
                    };
                    return wf;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName)
                    {
                        case "Date":
                            // Avoid exception on getting null in JSON for a non-null value type.
                            if (reader.TokenType == JsonTokenType.Null)
                            {
                                wf.Date = DateTimeOffset.Now;
                            }
                            else
                            {
                                DateTimeOffset date = reader.GetDateTimeOffset();
                                wf.Date = date;
                            }
                            break;
                        case "TemperatureCelsius":
                            int temperatureCelsius = reader.GetInt32();
                            wf.TemperatureCelsius = temperatureCelsius;
                            break;
                        case "Summary":
                            string summary = reader.GetString();
                            // Ignore properties in JSON based on criteria evaluated at runtime.
                            if (wf.TemperatureCelsius != 0)
                            {
                                wf.Summary = summary;
                            }
                            break;
                    }
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, WeatherForecast wf, JsonSerializerOptions options)
        {
            // Location for OnSerializing "callback" code.
            Console.WriteLine("OnSerializing");

            writer.WriteStartObject();

            writer.WriteString("Date", wf.Date);
            writer.WriteNumber("TemperatureCelsius", wf.TemperatureCelsius);
            if (wf.TemperatureCelsius != 0)
            {
                writer.WriteString("Summary", wf.Summary);
            }

            writer.WriteEndObject();

            // Location for Onserialized "callback" code.
            Console.WriteLine("OnSerialized");
        }
    }
}
