using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace SystemTextJsonSamples
{
    class Utf8ReaderFromFile
    {
        private static readonly byte[] s_nameUtf8 = Encoding.UTF8.GetBytes("name");
        private static readonly byte[] s_universityUtf8 = Encoding.UTF8.GetBytes("University");

        public static void Run()
        {
            // Read as UTF-16 and transcode to UTF-8 to convert to a Span<byte>
            string fileName = "Universities.json";
            string jsonString = File.ReadAllText(fileName);
            ReadOnlySpan<byte> jsonReadOnlySpan = Encoding.UTF8.GetBytes(jsonString);

            // Or ReadAllBytes if the file encoding is UTF-8:
            //string fileName = "UniversitiesUtf8.json";
            //ReadOnlySpan<byte> jsonReadOnlySpan = File.ReadAllBytes(fileName);

            int count = 0;
            int total = 0;

            var json = new Utf8JsonReader(jsonReadOnlySpan, isFinalBlock: true, state: default);

            while (json.Read())
            {
                JsonTokenType tokenType = json.TokenType;

                switch (tokenType)
                {
                    case JsonTokenType.StartObject:
                        total++;
                        break;
                    case JsonTokenType.PropertyName:
                        if (json.ValueSpan.SequenceEqual(s_nameUtf8))
                        {
                            // Assume valid JSON, known schema
                            json.Read();
                            if (json.ValueSpan.EndsWith(s_universityUtf8))
                            {
                                count++;
                            }
                        }
                        break;
                }
            }
            Console.WriteLine($"{count} out of {total} have names that end with 'University'");
        }
    }
}
