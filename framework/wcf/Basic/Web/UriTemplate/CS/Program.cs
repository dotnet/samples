//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Collections.Specialized;

namespace Microsoft.Samples.UriTemplates
{
    public class Program
    {
        public static void Main()
        {
            Uri prefix = new Uri("http://localhost/");

            //A UriTemplate is a "URI with holes". It describes a set of URI's that
            //are structurally similar. This UriTemplate might be used for organizing
            //weather reports:
            UriTemplate template = new UriTemplate("weather/{state}/{city}");

            //You can convert a UriTemplate into a Uri by filling
            //the holes in the template with parameters.

            //BindByPosition moves left-to-right across the template
            Uri positionalUri = template.BindByPosition(prefix, "Washington", "Redmond");

            Console.WriteLine("Calling BindByPosition...");
            Console.WriteLine(positionalUri);
            Console.WriteLine();

            //BindByName takes a NameValueCollection of parameters. 
            //Each parameter gets substituted into the UriTemplate "hole"
            //that has the same name as the parameter.
            NameValueCollection parameters = new NameValueCollection();
            parameters.Add("state", "Washington");
            parameters.Add("city", "Redmond");

            Uri namedUri = template.BindByName(prefix, parameters);

            Console.WriteLine("Calling BindByName...");
            Console.WriteLine(namedUri);
            Console.WriteLine();


            //The inverse operation of Bind is Match(), which extrudes a URI
            //through the template to produce a set of name/value pairs.
            Uri fullUri = new Uri("http://localhost/weather/Washington/Redmond");
            UriTemplateMatch results = template.Match(prefix, fullUri);

            Console.WriteLine(String.Format("Matching {0} to {1}", template.ToString(), fullUri.ToString()));

            if (results != null)
            {
                foreach (string variableName in results.BoundVariables.Keys)
                {
                    Console.WriteLine(String.Format("   {0}: {1}", variableName, results.BoundVariables[variableName]));
                }
            }

            Console.WriteLine("Press any key to terminate");
            Console.ReadLine();
        }
    }
}
