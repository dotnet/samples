//  Copyright (c) Microsoft Corporation. All rights reserved.

using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Microsoft.Samples.WebForms;

public partial class Default : System.Web.UI.Page
{
    private WeatherServiceClient client;

    protected void Page_Load(Object sender, EventArgs e)
    {
        // capture a reference to the cached client.  This reference will stay with this page for the
        // lifetime of the page.
        // we want to use this same instance of the client for calling BeginGetWeatherData and EndGetWeatherData
        // in order for any exceptions on the client to bubble up
        // if we didn't do this, there is a chance that the Global.Client instance could be
        // replaced and we may mask certain error conditions
        this.client = Global.Client;

        // This page is marked Async='true' so we need to 
        // call the service asynchronously to get the weather data
        client.GetWeatherDataCompleted += new EventHandler<GetWeatherDataCompletedEventArgs>(client_GetWeatherDataCompleted);
        string[] localities = { "Los Angeles", "Rio de Janeiro", "New York", "London", "Paris", "Rome", "Cairo", "Beijing" };
        client.GetWeatherDataAsync(localities);
    }

    void client_GetWeatherDataCompleted(object sender, GetWeatherDataCompletedEventArgs e)
    {
        dataGrid1.DataSource = e.Result;
        dataGrid1.DataBind();        
    }

}
