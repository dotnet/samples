//------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ServiceModel.Activation;
using System.ServiceModel.Syndication;
using System.Web.Routing;

namespace Microsoft.Samples.WebRoutingIntegration
{
    public class Global : System.Web.HttpApplication
    {
        static List<SyndicationItem> channels;

        public static List<SyndicationItem> Channels
        {
            get { return channels; }
            set { channels = value; }
        }
        
        static List<SyndicationItem> movies;

        public static List<SyndicationItem> Movies
        {
            get { return movies; }
            set { movies = value; }
        }
        
        protected void Application_Start(object sender, EventArgs e)
        {

            channels = Helpers.ReadTVChannelConfigurationFromDb();
            movies = Helpers.ReadMovieChannelConfigurationFromDb();
            RouteTable.Routes.Add(new ServiceRoute("movies", new WebServiceHostFactory(), typeof(Movies)));
            RouteTable.Routes.Add(new ServiceRoute("channels", new WebServiceHostFactory(), typeof(Channels)));

        }

        protected void Session_Start(object sender, EventArgs e)
        {
          
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}