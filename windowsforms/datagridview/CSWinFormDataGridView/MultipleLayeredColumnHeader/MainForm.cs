/************************************* Module Header **************************************\
* Module Name:  MultipleLayeredColumnHeader
* Project:      CSWinFormDataGridView
* Copyright (c) Microsoft Corporation.
*
*
* This sample demonstrates how to display multiple layer column headers on the DataGridView.
\******************************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CSWinFormDataGridView.MultipleLayeredColumnHeader
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.dataGridView1.Columns.Add("JanWin", "Win");
            this.dataGridView1.Columns.Add("JanLoss", "Loss");
            this.dataGridView1.Columns.Add("FebWin", "Win");
            this.dataGridView1.Columns.Add("FebLoss", "Loss");
            this.dataGridView1.Columns.Add("MarWin", "Win");
            this.dataGridView1.Columns.Add("MarLoss", "Loss");

            for (int j = 0; j < this.dataGridView1.ColumnCount; j++)
            {
                this.dataGridView1.Columns[j].Width = 45;
            }

            // Enable resizing on the column headers;
            this.dataGridView1.ColumnHeadersHeightSizeMode =
                 DataGridViewColumnHeadersHeightSizeMode.EnableResizing;

            // Adjust the height for the column headers
            this.dataGridView1.ColumnHeadersHeight =
                        this.dataGridView1.ColumnHeadersHeight * 2;

            // Adjust the text alignment on the column headers to make the text display
            // at the center of the bottom;
            this.dataGridView1.ColumnHeadersDefaultCellStyle.Alignment =
                 DataGridViewContentAlignment.BottomCenter;

            // Handle the CellPainting event to draw text for each header cell
            this.dataGridView1.CellPainting += new
                 DataGridViewCellPaintingEventHandler(dataGridView1_CellPainting);

            // Handle the Paint event to draw "merged" header cells;
            this.dataGridView1.Paint += new PaintEventHandler(dataGridView1_Paint);
        }

        void dataGridView1_Paint(object sender, PaintEventArgs e)
        {
            // Data for the "merged" header cells
            string[] monthes = { "January", "February", "March" };

            for (int j = 0; j < this.dataGridView1.ColumnCount;)
            {
                // Get the column header cell bounds
                Rectangle r1 = this.dataGridView1.GetCellDisplayRectangle(j, -1, true);

                r1.X += 1;
                r1.Y += 1;
                r1.Width = r1.Width * 2 - 2;
                r1.Height = r1.Height / 2 - 2;

                using (SolidBrush br =
                    new SolidBrush(
                        this.dataGridView1.ColumnHeadersDefaultCellStyle.BackColor))
                {
                    e.Graphics.FillRectangle(br, r1);
                }

                using (Pen p = new Pen(SystemColors.InactiveBorder))
                {
                    e.Graphics.DrawLine(p, r1.X, r1.Bottom, r1.Right, r1.Bottom);
                }

                using (StringFormat format = new StringFormat())
                using (SolidBrush br =
                    new SolidBrush(
                        this.dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor))
                {
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;

                    e.Graphics.DrawString(monthes[j / 2],
                        this.dataGridView1.ColumnHeadersDefaultCellStyle.Font,
                        br,
                        r1,
                        format);
                }

                j += 2;
            }
        }

        void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex > -1)
            {
                e.PaintBackground(e.CellBounds, false);

                Rectangle r2 = e.CellBounds;
                r2.Y += e.CellBounds.Height / 2;
                r2.Height = e.CellBounds.Height / 2;
                e.PaintContent(r2);

                e.Handled = true;
            }
        }
    }
}
