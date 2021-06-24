//  Copyright (c) Microsoft Corporation. All rights reserved.

using System;
using System.ServiceModel.Channels;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Microsoft.Samples.WindowsForms
{
    public partial class Form1 : Form
    {
        // keep the client around for the lifetime of the form
        WeatherServiceClient client = new WeatherServiceClient();

        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            //Closing the client gracefully closes the connection and cleans up resources
            client.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string[] localities = { "Los Angeles", "Rio de Janeiro", "New York", "London", "Paris", "Rome", "Cairo", "Beijing" };
            client.GetWeatherDataCompleted += new EventHandler<GetWeatherDataCompletedEventArgs>(client_GetWeatherDataCompleted);
            client.GetWeatherDataAsync(localities);
        }

        void client_GetWeatherDataCompleted(object sender, GetWeatherDataCompletedEventArgs e)
        {
            BindingSource bSource = new BindingSource();
            bSource.DataSource = e.Result;
            dataGridView1.DataSource = bSource;
        }

    }
}
