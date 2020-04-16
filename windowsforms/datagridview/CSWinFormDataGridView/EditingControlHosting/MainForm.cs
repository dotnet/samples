/************************************* Module Header **************************************\
* Module Name:  EditingControlHosting
* Project:      CSWinFormDataGridView
* Copyright (c) Microsoft Corporation.
*
* This sample demonstrates how to host a control in the current DataGridViewCell  for
* editing.
\******************************************************************************************/

#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
#endregion

namespace CSWinFormDataGridView.EditingControlHosting
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private MaskedTextBox maskedTextBoxForEditing;
        private bool IsKeyPressHandled = false;

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.maskedTextBoxForEditing = new MaskedTextBox();

            // The "000-00-0000" mask allows only digits can be input
            this.maskedTextBoxForEditing.Mask = "000-00-0000";
            // this.maskedTextBoxForEditing.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;

            // Hide the MaskedTextBox
            this.maskedTextBoxForEditing.Visible = false;

            // Add the MaskedTextBox to the DataGridView's control collection
            this.dataGridView1.Controls.Add(this.maskedTextBoxForEditing);

            // Add a DataGridViewTextBoxColumn to the
            DataGridViewTextBoxColumn tc = new DataGridViewTextBoxColumn();
            tc.HeaderText = "Mask Column";
            tc.Name = "MaskColumn";
            this.dataGridView1.Columns.Add(tc);

            // Add some empty rows for testing purpose.
            for (int j = 0; j < 30; j++)
            {
                this.dataGridView1.Rows.Add();
            }

            // Handle the CellBeginEdit event to show the MaskedTextBox on
            // the current editing cell
            this.dataGridView1.CellBeginEdit +=
                new DataGridViewCellCancelEventHandler(dataGridView1_CellBeginEdit);

            // Handle the CellEndEdit event to hide the MaskedTextBox when
            // editing completes.
            this.dataGridView1.CellEndEdit += new DataGridViewCellEventHandler(dataGridView1_CellEndEdit);

            // Handle the Scroll event to adjust the location of the MaskedTextBox as it is showing
            // when scrolling the DataGridView
            this.dataGridView1.Scroll += new ScrollEventHandler(dataGridView1_Scroll);

            // Handle the EditingControlShowing event to pass the focus to the
            // MaskedTextBox when begin editing with keystrokes
            this.dataGridView1.EditingControlShowing +=
                new DataGridViewEditingControlShowingEventHandler(dataGridView1_EditingControlShowing);
        }

        void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // If the current cell is on the "MaskColumn", we use the MaskedTextBox control
            // for editing instead of the default TextBox control;
            if (e.ColumnIndex == this.dataGridView1.Columns["MaskColumn"].Index)
            {
                // Calculate the cell bounds of the current cell
                Rectangle rect = this.dataGridView1.GetCellDisplayRectangle(
                    e.ColumnIndex, e.RowIndex, true);
                // Adjust the MaskedTextBox's size and location to fit the cell
                this.maskedTextBoxForEditing.Size = rect.Size;
                this.maskedTextBoxForEditing.Location = rect.Location;

                // Set value for the MaskedTextBox
                if (this.dataGridView1.CurrentCell.Value != null)
                {
                    this.maskedTextBoxForEditing.Text = this.dataGridView1.CurrentCell.Value.ToString();
                }

                // Show the MaskedTextBox
                this.maskedTextBoxForEditing.Visible = true;
            }
        }

        void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // When finish editing on the "MaskColumn", we replace the cell value with
            // the text typed in the MaskedTextBox, and hide the MaskedTextBox;
            if (e.ColumnIndex == this.dataGridView1.Columns["MaskColumn"].Index)
            {
                this.dataGridView1.CurrentCell.Value = this.maskedTextBoxForEditing.Text;
                this.maskedTextBoxForEditing.Text = "";
                this.maskedTextBoxForEditing.Visible = false;
            }
        }

        void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            if (this.dataGridView1.IsCurrentCellInEditMode == true)
            {
                // Adjust the location for the MaskedTextBox while scrolling
                Rectangle rect = this.dataGridView1.GetCellDisplayRectangle(
                    this.dataGridView1.CurrentCell.ColumnIndex,
                    this.dataGridView1.CurrentCell.RowIndex, true);

                Console.WriteLine(rect.ToString());
                Console.WriteLine(this.dataGridView1.CurrentCellAddress.ToString());
                Console.WriteLine("");

                if (rect.X <= 0 || rect.Y <= 0)
                {
                    this.maskedTextBoxForEditing.Visible = false;
                }
                else
                {
                    this.maskedTextBoxForEditing.Location = rect.Location;
                }
            }
        }

        void dataGridView1_EditingControlShowing(object sender,
            DataGridViewEditingControlShowingEventArgs e)
        {
            if (!this.IsKeyPressHandled
                && this.dataGridView1.CurrentCell.ColumnIndex ==
                this.dataGridView1.Columns["MaskColumn"].Index)
            {
                TextBox tb = e.Control as TextBox;
                tb.KeyPress += new KeyPressEventHandler(tb_KeyPress);
                this.IsKeyPressHandled = true;
            }
        }

        void tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (this.dataGridView1.CurrentCell.ColumnIndex ==
                this.dataGridView1.Columns["MaskColumn"].Index)
            {
                // Prevent the key char to be input in the editing control
                e.Handled = true;

                // Set focus to the MaskedTextBox for editing.
                this.maskedTextBoxForEditing.Focus();
            }
        }
    }
}
