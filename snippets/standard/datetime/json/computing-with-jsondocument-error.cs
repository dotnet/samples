using System;
using System.Text.Json;

public class Example
{
    private static double ComputeAverageTemperatures(string json)
    {
        var options = new JsonDocumentOptions
        {
            AllowTrailingCommas = true
        };

        using (JsonDocument document = JsonDocument.Parse(json, options))
        {
            int sumOfAllTemperatures = 0;
            int count = 0;

            foreach (JsonElement element in document.RootElement.EnumerateArray())
            {
                DateTimeOffset date = element.GetProperty("date").GetDateTimeOffset();

                if (date.DayOfWeek == DayOfWeek.Monday)
                {
                    int temp = element.GetProperty("temp").GetInt32();
                    sumOfAllTemperatures += temp;
                    count++;
                }
            }

            var averageTemp = (double)sumOfAllTemperatures / count;
            return averageTemp;
        }
    }

    public static void Main(string[] args)
    {
        string json =
                @"[" +
                    @"{" +
                        @"""date"": ""2013/01/07 00:00:00Z""," +
                        @"""temp"": 23," +
                    @"}," +
                    @"{" +
                        @"""date"": ""2013/01/08 00:00:00Z""," +
                        @"""temp"": 28," +
                    @"}," +
                    @"{" +
                        @"""date"": ""2013/01/14 00:00:00Z""," +
                        @"""temp"": 8," +
                    @"}," +
                @"]";

        Console.WriteLine(ComputeAverageTemperatures(json));
    }
}

// The example displays the following output:
// Unhandled exception.System.FormatException: One of the identified items was in an invalid format.
//    at System.Text.Json.JsonElement.GetDateTimeOffset()
//    at Example.ComputeAverageTemperatures(String json) in D:\console_apps\DateTimeConverterExamples\Program.cs:line 20
//    at Example.Main(String[] args)
