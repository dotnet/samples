/********************************* Module Header **********************************\
* Module Name:  JustInTimeDataLoading
* Project:      CSWinFormDataGridView
* Copyright (c) Microsoft Corporation.
*
* This sample demonstrates how to use virtual mode in the DataGridView control
* with a data cache that loads data from a server only when it is needed.
* This kind of data loading is called "Just-in-time data loading".
\**********************************************************************************/

#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
#endregion

namespace CSWinFormDataGridView.JustInTimeDataLoading
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private Cache memoryCache;

        // Specify a connection string. Replace the given value with a
        // valid connection string for a Northwind SQL Server sample
        // database accessible to your system.
        private string connectionString =
            "Initial Catalog=NorthWind;Data Source=localhost;" +
            "Integrated Security=SSPI;Persist Security Info=False";

        private string table = "Orders";

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Enable VirtualMode on the DataGridView
            this.dataGridView1.VirtualMode = true;

            // Handle the CellValueNeeded event to retrieve the requested cell value
            // from the data store or the Customer object currently in edit.
            // This event occurs whenever the DataGridView control needs to paint a cell.
            this.dataGridView1.CellValueNeeded += new
                DataGridViewCellValueEventHandler(dataGridView1_CellValueNeeded);

            // Create a DataRetriever and use it to create a Cache object
            // and to initialize the DataGridView columns and rows.
            try
            {
                DataRetriever retriever =
                    new DataRetriever(connectionString, table);
                memoryCache = new Cache(retriever, 16);
                foreach (DataColumn column in retriever.Columns)
                {
                    dataGridView1.Columns.Add(
                        column.ColumnName, column.ColumnName);
                }
                this.dataGridView1.RowCount = retriever.RowCount;
            }
            catch (SqlException)
            {
                MessageBox.Show("Connection could not be established. " +
                    "Verify that the connection string is valid.");
                Application.Exit();
            }
        }

        private void dataGridView1_CellValueNeeded(object sender,
            DataGridViewCellValueEventArgs e)
        {
            e.Value = memoryCache.RetrieveElement(e.RowIndex, e.ColumnIndex);
        }
    }
}
