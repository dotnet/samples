using System;
using System.Collections.Generic;
using System.Text;

namespace whats_new
{
    public class JSON
    {
        public string WriteJSON()
        {
            // <SnippetJsonSerialize>
            var instance = new
            {
                FirstName = "John",
                LastName = "Smith",
                Age = 18
            };
            
            System.Console.WriteLine(System.Text.Json.Serialization.JsonSerializer.ToString(instance));
            // </SnippetJsonSerialize>

            return System.Text.Json.Serialization.JsonSerializer.ToString(instance);
        }
    }

    // <SnippetJsonDeserialize>
    public class JsonPerson
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }

        public static JsonPerson Parse(string json) =>
            System.Text.Json.Serialization.JsonSerializer.Parse<JsonPerson>(json);
    }
    // </SnippetJsonDeserialize>
}
