//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.Samples.UriTemplateDispatcher
{
    public class Program
    {
        delegate void Handler(UriTemplateMatch results);

        public static void Main()
        {
            Uri prefix = new Uri("http://localhost/");

            //One interesting use for UriTemplateTable is as a dispatching engine.
            //This is accomplished by using the UriTemplateTable to associate
            //a delegate handler with each UriTemplate.

            //To start, we need a UriTemplateTable.
            UriTemplateTable table = new UriTemplateTable(prefix);

            //Now, we add templates to the table and associate them
            //with handler functions, written as anonymous delegates:

            AddHandler(table, new UriTemplate("weather/{state}/{city}"),
                delegate(UriTemplateMatch r)
            {
                Console.WriteLine("Matched the WeatherByCity template");
                Console.WriteLine("State: {0}", r.BoundVariables["state"]);
                Console.WriteLine("City: {0}", r.BoundVariables["city"]);
            });

            AddHandler(table, new UriTemplate("weather/{state}"),
                delegate(UriTemplateMatch r)
            {
                Console.WriteLine("Matched the WeatherByState template");
                Console.WriteLine("State: {0}", r.BoundVariables["state"]);
            });

            AddHandler(table, new UriTemplate("traffic/*"),
                delegate(UriTemplateMatch r)
            {
                Console.WriteLine("Matched the traffic/* template");
                Console.WriteLine("Wildcard segments:");

                foreach (string s in r.WildcardPathSegments)
                {
                    Console.WriteLine("   " + s);
                }
            });

            //MakeReadOnly() freezes the table and prevents further additions.
            //Passing false to this method will disallow duplicate URI's,
            //guaranteeing that at most a single match will be returned. 
            //Calling this method explictly is optional, as the collection
            //will be made read-only during the first call to Match() or MatchSingle()
            table.MakeReadOnly(false);

            Uri request = null;

            //Match WeatherByCity
            request = new Uri("http://localhost/weather/Washington/Seattle");
            Console.WriteLine(request);
            InvokeDelegate(table.MatchSingle(request));

            //Match WeatherByState
            request = new Uri("http://localhost/weather/Washington");
            Console.WriteLine(request);
            InvokeDelegate(table.MatchSingle(request));

            //Wildcard match Traffic
            request = new Uri("http://localhost/Traffic/Washington/Seattle/SR520");
            Console.WriteLine(request);
            InvokeDelegate(table.MatchSingle(request));

            Console.WriteLine("Press any key to terminate");
            Console.ReadLine();
        }

        private static void AddHandler(UriTemplateTable table, UriTemplate template, Handler handler)
        {
            table.KeyValuePairs.Add(new KeyValuePair<UriTemplate, object>(template, handler));
        }

        private static void InvokeDelegate(UriTemplateMatch results)
        {
            if (results == null)
            {
                Console.WriteLine("No Match");
            }
            else
            {
                Handler handler = (Handler)(results.Data);
                handler(results);
            }

            Console.WriteLine("");
        }
    }
}
