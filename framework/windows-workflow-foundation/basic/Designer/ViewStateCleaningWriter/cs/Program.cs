//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------


using System;
using System.Activities;
using System.Activities.XamlIntegration;
using System.IO;
using System.Xaml;
using System.Xml;

namespace Microsoft.Samples.Activities.Designer.ViewStateCleaner
{
    class Program
    {
        static void Main(string[] args)
        {
            if (2 != args.Length)
            {
                Console.WriteLine("Usage -- ViewStateCleaningWriter <infile> <outfile>");
                return;
            }
            try
            {
                XmlReader xmlReader = XmlReader.Create(args[0]);
                XamlXmlReader xamlReader = new XamlXmlReader(xmlReader);
                ActivityBuilder ab = XamlServices.Load(ActivityXamlServices.CreateBuilderReader(xamlReader)) as ActivityBuilder;

                XmlWriterSettings writerSettings = new XmlWriterSettings {  Indent = true };
                XmlWriter xmlWriter = XmlWriter.Create(File.OpenWrite(args[1]), writerSettings);
                XamlXmlWriter xamlWriter = new XamlXmlWriter(xmlWriter, new XamlSchemaContext());
                XamlServices.Save(new ViewStateCleaningWriter(ActivityXamlServices.CreateBuilderWriter(xamlWriter)), ab);

                Console.WriteLine("{0} written without viewstate information", args[1]);

            }
            catch (Exception ex)
            {

                Console.WriteLine("Exception encountered {0}", ex);
            }

        }
    }
}
