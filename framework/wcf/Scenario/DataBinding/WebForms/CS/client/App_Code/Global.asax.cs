//  Copyright (c) Microsoft Corporation. All rights reserved.

using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Microsoft.Samples.WebForms;
using System.ServiceModel;
using System.IO;

public class Global : HttpApplication
{
    static protected WeatherServiceClient client;
    private static object clientLock = new Object();

    // We only want one client per instance of the app because it is inefficient 
    // to create a new client for every request.
    static public WeatherServiceClient Client
    {
        get
        {
            // lazy init the client
            lock (clientLock)
            {
                if (client == null)
                {
                    client = new WeatherServiceClient();
                }
                return client;
            }
        }
        set
        {
            lock (clientLock)
            {
                client = value;
            }
        }
    }

    // Our client is scoped to the lifetime of the HttpApplication so
    // when the Application_End method is invoked, we should call Close on the client
    public void Application_End(object sender, EventArgs e)
    {
        WeatherServiceClient localClient = client;

        if (localClient != null && localClient.State == System.ServiceModel.CommunicationState.Opened)
        {
            localClient.Close();
        }
    }
}
