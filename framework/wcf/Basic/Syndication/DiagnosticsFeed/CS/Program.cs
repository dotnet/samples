//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved. 
//----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Syndication;
using System.ServiceModel.Web;
using System.Xml;

namespace Microsoft.Samples.DiagnosticsFeed
{
    [ServiceContract(Namespace = "")]
    interface IDiagnosticsService
    {
        //The [WebGet] attribute controls how WCF dispatches
        //HTTP requests to service operations based on a URI suffix
        //(the part of the request URI after the endpoint address)
        //using the HTTP GET method. The UriTemplate specifies a relative
        //path of 'feed', and specifies that the format is
        //supplied using a query string. 
        [WebGet(UriTemplate="feed?format={format}")]
        [ServiceKnownType(typeof(Atom10FeedFormatter))]
        [ServiceKnownType(typeof(Rss20FeedFormatter))]
        SyndicationFeedFormatter GetProcesses(string format);
    }

    public class ProcessService : IDiagnosticsService
    {
        public SyndicationFeedFormatter GetProcesses(string format)
        {
            IEnumerable<Process> processes = new List<Process>( Process.GetProcesses() );

            //SyndicationFeed also has convenience constructors
            //that take in common elements like Title and Content.
            SyndicationFeed f = new SyndicationFeed();            

            
            f.LastUpdatedTime = DateTimeOffset.Now;
            f.Title = SyndicationContent.CreatePlaintextContent("Currently running processes");
            f.Links.Add(SyndicationLink.CreateSelfLink(OperationContext.Current.IncomingMessageHeaders.To));

            f.Items = from p in processes
                      select new SyndicationItem()
                      {
                          LastUpdatedTime = DateTimeOffset.Now,
                          Title = SyndicationContent.CreatePlaintextContent(p.ProcessName),
                          Summary = SyndicationContent.CreateHtmlContent(String.Format("<b>{0}</b>", p.MainWindowTitle)),
                          Content = SyndicationContent.CreateXmlContent(new ProcessData(p))
                      };

            
            // Choose a formatter according to the query string passed.
            if (format == "rss")
            {
                return new Rss20FeedFormatter(f);
            }
            else
            {
                return new Atom10FeedFormatter(f);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //Expand this region to see the code for hosting a Syndication Feed.
            #region Service

            WebServiceHost host = new WebServiceHost(typeof(ProcessService), new Uri("http://localhost:8000/diagnostics"));

            //The WebServiceHost will automatically provide a default endpoint at the base address
            //using the proper binding (the WebHttpBinding) and endpoint behavior (the WebHttpBehavior)
            host.Open();

            Console.WriteLine("Service host open");

            //The syndication feeds exposed by the service are available
            //at the following URI (for Atom use feed/?format=atom)

            // http://localhost:8000/diagnostics/feed/?format=rss

            //These feeds can be viewed directly in an RSS-aware client
            //such as IE7.
            #endregion

            //Expand this region to see the code for accessing a Syndication Feed.
            #region Client

            // Change the value of the feed query string to 'atom' to use Atom format.
            XmlReader reader = XmlReader.Create( "http://localhost:8000/diagnostics/feed?format=rss",
                                                  new XmlReaderSettings()
                                                  {
                                                       //MaxCharactersInDocument can be used to control the maximum amount of data 
                                                       //read from the reader and helps prevent OutOfMemoryException
                                                       MaxCharactersInDocument = 1024 * 64
                                                  } );


            SyndicationFeed feed = SyndicationFeed.Load( reader );

            foreach (SyndicationItem i in feed.Items)
            {
                XmlSyndicationContent content = i.Content as XmlSyndicationContent;
                ProcessData pd = content.ReadContent<ProcessData>();

                Console.WriteLine(i.Title.Text);
                Console.WriteLine(pd.ToString());
            }
            #endregion

            //Press any key to end the program.
            Console.ReadLine();
        }
    }
}
