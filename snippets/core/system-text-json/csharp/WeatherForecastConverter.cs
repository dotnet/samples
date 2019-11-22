using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SystemTextJsonSamples
{
    public class WeatherForecastConverter : JsonConverter<WeatherForecast>
    {
        public override WeatherForecast Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
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
                    Console.WriteLine("OnDeserialized");
                    return wf;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName)
                    {
                        case "Date":
                            if (reader.TokenType == JsonTokenType.Null)
                            {
                                wf.Date = DateTimeOffset.MinValue;
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
            Console.WriteLine("OnSerializing");

            writer.WriteStartObject();

            writer.WriteString("Date", wf.Date);
            writer.WriteNumber("TemperatureCelsius", wf.TemperatureCelsius);
            if (wf.TemperatureCelsius != 0)
            {
                writer.WriteString("Summary", wf.Summary);
            }

            writer.WriteEndObject();

            Console.WriteLine("OnSerialized");

        }
    }
}
