/********************************* Module Header **********************************\
* Module Name:  DataGridViewPaging
* Project:      CSWinFormDataGridView
* Copyright (c) Microsoft Corporation.
*
* This sample demonstrates how to page data in the  DataGridView control;
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

namespace CSWinFormDataGridView.DataGridViewPaging
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private int PageSize = 30; // 30 rows per page
        private int CurrentPageIndex = 1;
        private int TotalPage;

        private string connstr =
            "Persist Security Info=False;" +
            "Integrated Security=SSPI;" +
            "Initial Catalog=Northwind;" +
            "server=localhost";

        private SqlConnection conn;
        private SqlDataAdapter adapter;
        private SqlCommand command;

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.conn = new SqlConnection(connstr);
            this.adapter = new SqlDataAdapter();
            this.command = conn.CreateCommand();

            // Get total count of the pages;
            this.GetTotalPageCount();

            this.dataGridView1.ReadOnly = true;

            // Load the first page of data;
            this.dataGridView1.DataSource = GetPageData(1);
        }

        private void GetTotalPageCount()
        {
            command.CommandText = "Select Count(OrderID) From Orders";

            try
            {
                conn.Open();
                int rowCount = (int)command.ExecuteScalar();

                this.TotalPage = rowCount / PageSize;

                if (rowCount % PageSize > 0)
                {
                    this.TotalPage += 1;
                }
            }
            finally
            {
                conn.Close();
            }
        }

        private DataTable GetPageData(int page)
        {
            DataTable dt = new DataTable();

            if (page == 1)
            {
                command.CommandText =
                    "Select Top " + PageSize + " * From Orders Order By OrderID";
            }
            else
            {
                int lowerPageBoundary = (page - 1) * PageSize;

                command.CommandText = "Select Top " + PageSize +
                    " * From Orders " +
                    " WHERE OrderID NOT IN " +
                    " (SELECT TOP " + lowerPageBoundary + " OrderID From Orders Order By OrderID) " +
                    " Order By OrderID";
            }
            try
            {
                this.conn.Open();
                this.adapter.SelectCommand = command;
                this.adapter.Fill(dt);
            }
            finally
            {
                conn.Close();
            }

            return dt;
        }

        private void toolStripButtonFirst_Click(object sender, EventArgs e)
        {
            this.CurrentPageIndex = 1;
            this.dataGridView1.DataSource = GetPageData(this.CurrentPageIndex);
        }

        private void toolStripButtonPrev_Click(object sender, EventArgs e)
        {
            if (this.CurrentPageIndex > 1)
            {
                this.CurrentPageIndex--;
                this.dataGridView1.DataSource = GetPageData(this.CurrentPageIndex);
            }
        }

        private void toolStripButtonNext_Click(object sender, EventArgs e)
        {
            if (this.CurrentPageIndex < this.TotalPage)
            {
                this.CurrentPageIndex++;
                this.dataGridView1.DataSource = GetPageData(this.CurrentPageIndex);
            }
        }

        private void toolStripButtonLast_Click(object sender, EventArgs e)
        {
            this.CurrentPageIndex = TotalPage;
            this.dataGridView1.DataSource = GetPageData(this.CurrentPageIndex);
        }
    }
}
