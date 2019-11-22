using System;
using System.Threading.Tasks;

namespace SystemTextJsonSamples
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("\n============================= Roundtrip to string\n");
            RoundtripToString.Run();

            Console.WriteLine("\n============================= Roundtrip to UTF-8 byte array\n");
            RoundtripToUtf8.Run();

            Console.WriteLine("\n============================= Roundtrip to file\n");
            RoundtripToFile.Run();

            Console.WriteLine("\n============================= Roundtrip to file async\n");
            await RoundtripToFileAsync.RunAsync();

            Console.WriteLine("\n============================= Roundtrip camel case property names\n");
            RoundtripCamelCasePropertyNames.Run();

            Console.WriteLine("\n============================= Roundtrip custom property naming policy\n");
            RoundtripPropertyNamingPolicy.Run();

            Console.WriteLine("\n============================= Roundtrip custom property name by attribute\n");
            RoundtripPropertyNamesByAttribute.Run();

            Console.WriteLine("\n============================= Roundtrip extension data\n");
            RoundtripExtensionData.Run();

            Console.WriteLine("\n============================= Roundtrip enum as string\n");
            RoundtripEnumAsString.Run();

            Console.WriteLine("\n============================= Serialize polymorphic\n");
            SerializePolymorphic.Run();

            Console.WriteLine("\n============================= Serialize custom encoding\n");
            SerializeCustomEncoding.Run();

            Console.WriteLine("\n============================= Serialize exclude null value properties\n");
            SerializeExcludeNullValueProperties.Run();

            Console.WriteLine("\n============================= Serialize exclude properties by attribute\n");
            SerializeExcludePropertiesByAttribute.Run();

            Console.WriteLine("\n============================= Serialize exclude read-only properties\n");
            SerializeExcludeReadOnlyProperties.Run();

            Console.WriteLine("\n============================= Serialize camel case Dictionary keys\n");
            SerializeCamelCaseDictionaryKeys.Run();

            Console.WriteLine("\n============================= Deserialize case-insensitive\n");
            DeserializeCaseInsensitive.Run();

            Console.WriteLine("\n============================= Deserialize ignore null\n");
            DeserializeIgnoreNull.Run();

            Console.WriteLine("\n============================= Deserialize trailing commas and comments\n");
            DeserializeCommasComments.Run();

            Console.WriteLine("\n============================= Custom converter registration - Converters collection\n");
            RegisterConverterWithConverterscollection.Run();

            Console.WriteLine("\n============================= Custom converter registration - Converters attribute on property\n");
            RegisterConverterWithAttributeOnProperty.Run();

            Console.WriteLine("\n============================= Custom converter registration - attribute on type\n");
            RegisterConverterWithAttributeOnType.Run();

            Console.WriteLine("\n============================= Custom converter Dictionary with TKey = Enum\n");
            ConvertDictionaryTkeyEnumTValue.Run();

            Console.WriteLine("\n============================= Custom converter Polymorphic\n");
            ConvertPolymorphic.Run();

            Console.WriteLine("\n============================= Custom converter inferred types to Object\n");
            ConvertInferredTypesToObject.Run();

            Console.WriteLine("\n============================= Callbacks\n");
            Callbacks.Run();

            Console.WriteLine("\n============================= JsonDocument data access\n");
            JsonDocumentDataAccess.Run();

            Console.WriteLine("\n============================= JsonDocument write JSON\n");
            JsonDocumentWriteJson.Run();

            Console.WriteLine("\n============================= Utf8Reader from file\n");
            Utf8ReaderFromFile.Run();

            Console.WriteLine("\n============================= Utf8Reader from byte array\n");
            Utf8ReaderFromBytes.Run();

            Console.WriteLine("\n============================= Utf8Writer to Stream\n");
            Utf8WriterToStream.Run();
        }
    }
}
