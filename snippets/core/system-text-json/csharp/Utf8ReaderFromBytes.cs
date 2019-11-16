using System;
using System.Text.Json;

namespace SystemTextJsonSamples
{
    class Utf8ReaderFromBytes
    {
        public static void Run()
        {
            var weatherForecast = WeatherForecastFactories.CreateWeatherForecast();
            byte[] jsonUtf8Bytes = JsonSerializer.SerializeToUtf8Bytes(
                weatherForecast,
                new JsonSerializerOptions { WriteIndented = true });

            // <SnippetDeserialize>
            var options = new JsonReaderOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip
            };
            Utf8JsonReader reader = new Utf8JsonReader(jsonUtf8Bytes, options);

            while (reader.Read())
            {
                Console.Write(reader.TokenType);

                switch (reader.TokenType)
                {
                    case JsonTokenType.PropertyName:
                    case JsonTokenType.String:
                        {
                            string text = reader.GetString();
                            Console.Write(" ");
                            Console.Write(text);
                            break;
                        }

                    case JsonTokenType.Number:
                        {
                            int value = reader.GetInt32();
                            Console.Write(" ");
                            Console.Write(value);
                            break;
                        }

                        // Other token types elided for brevity
                }
                Console.WriteLine();
            }
            // </SnippetDeserialize>
        }
    }
}
