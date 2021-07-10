//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.Samples.UriTemplateTables
{
    public class Program
    {
        public static void Main()
        {
            Uri prefix = new Uri("http://localhost/");

            //A UriTemplateTable is an associative set of UriTemplates. It lets you match
            //candidate URI's against the templates in the set, and retrieve the data associated
            //with the matching templates.

            //To start, we need some example templates and a table to put them in:
            UriTemplate weatherByCity = new UriTemplate("weather/{state}/{city}");
            UriTemplate weatherByState = new UriTemplate("weather/{state}");
            UriTemplate traffic = new UriTemplate("traffic/*");
            UriTemplate wildcard = new UriTemplate("*");

            UriTemplateTable table = new UriTemplateTable(prefix);

            //Now we associate each template with some data. Here we're just using strings;
            //you can associate anything you want with the template.
            table.KeyValuePairs.Add(new KeyValuePair<UriTemplate, object>(weatherByCity, "weatherByCity"));
            table.KeyValuePairs.Add(new KeyValuePair<UriTemplate, object>(weatherByState, "weatherByState"));
            table.KeyValuePairs.Add(new KeyValuePair<UriTemplate, object>(traffic, "traffic"));

            //Once the table is populated, we can use the MatchSingle function to
            //retrieve some match results:

            UriTemplateMatch results = null;
            Uri weatherInSeattle = new Uri("http://localhost/weather/Washington/Seattle");

            //The Match function will select the best match for the incoming URI 
            //based on the templates in the table. There are two variants of Match --
            //Match() can potentially return multiple results, which MatchSingle() will
            //either return exactly one result or throw an exception.
            
            results = table.MatchSingle(weatherInSeattle);

            //We get back the associated data in the Data member of
            //the UriTemplateMatchResults that are returned.
            if (results != null)
            {
                //Output will be "weatherByCity"
                Console.WriteLine(results.Data);

                //The UriTemplateMatch contains useful data obtained during the
                //matching process.

                Console.WriteLine("State: {0}", results.BoundVariables["state"]);
                Console.WriteLine("City: {0}", results.BoundVariables["city"]);
            }
            
            Console.WriteLine("Press any key to terminate");
            Console.ReadLine();
        }
    }

}
