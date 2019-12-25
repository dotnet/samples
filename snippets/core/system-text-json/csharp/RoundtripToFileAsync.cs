using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SystemTextJsonSamples
{
    class RoundtripToFileAsync
    {
        public static async Task RunAsync()
        {
            string fileName = "WeatherForecastAsync.json";
            string fileNameUtf8 = "WeatherForecastAsyncUtf8";
            WeatherForecast weatherForecast = WeatherForecastFactories.CreateWeatherForecast();
            weatherForecast.DisplayPropertyValues();

            // <SnippetSerialize>
            using (FileStream fs = File.Create(fileName))
            {
                await JsonSerializer.SerializeAsync(fs, weatherForecast);
            }
            // </SnippetSerialize>
            Console.WriteLine($"The result is in {fileName}\n");

            // <SnippetDeserialize>
            using (FileStream fs = File.OpenRead(fileName))
            {
                weatherForecast = await JsonSerializer.DeserializeAsync<WeatherForecast>(fs);
            }
            // </SnippetDeserialize>
            weatherForecast.DisplayPropertyValues();

            using (FileStream fs = File.Create(fileNameUtf8))
            {
                using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
                {
                    await JsonSerializer.SerializeAsync(fs, weatherForecast);
                }
            }
            Console.WriteLine($"The result is in {fileNameUtf8}\n");
            using (FileStream fs = File.OpenRead(fileNameUtf8))
            {
                //System.Text.Json.JsonException: '0xEF' is invalid after a single JSON value. Expected end of data.
                //weatherForecast = await JsonSerializer.DeserializeAsync<WeatherForecast>(fs);
            }

            weatherForecast.DisplayPropertyValues();
        }
    }
}
