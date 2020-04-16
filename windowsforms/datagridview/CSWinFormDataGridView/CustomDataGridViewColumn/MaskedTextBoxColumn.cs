/********************************* Module Header **********************************\
* Module Name:  MaskedTextBoxColumn.cs
* Project:      CSWinFormDataGridView
* Copyright (c) Microsoft Corporation.
*
* This sample demonstrates how to create a custom DataGridView column.
\**********************************************************************************/

#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
#endregion

namespace CSWinFormDataGridView.CustomDataGridViewColumn
{
    /// <summary>
    /// The base object for the custom column type.  Programmers manipulate
    /// the column types most often when working with the DataGridView, and
    /// this one sets the basics and Cell Template values controlling the
    /// default behaviour for cells of this column type.
    /// </summary>
    public class MaskedTextBoxColumn : DataGridViewColumn
    {
        private string mask;
        private char promptChar;
        private bool includePrompt;
        private bool includeLiterals;
        private Type validatingType;

        /// <summary>
        /// Initializes a new instance of this class, making sure to pass
        /// to its base constructor an instance of a MaskedTextBoxCell
        /// class to use as the basic template.
        /// </summary>
        public MaskedTextBoxColumn()
            : base(new MaskedTextBoxCell())
        {
        }

        /// <summary>
        /// Routine to convert from boolean to DataGridViewTriState.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static DataGridViewTriState TriBool(bool value)
        {
            return value ? DataGridViewTriState.True
                         : DataGridViewTriState.False;
        }

        /// <summary>
        /// The template cell that will be used for this column by default,
        /// unless a specific cell is set for a particular row.
        ///
        /// A MaskedTextBoxCell cell which will serve as the template cell
        /// for this column.
        /// </summary>
        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }

            set
            {
                //  Only cell types that derive from MaskedTextBoxCell are supported
                // as the cell template.
                if (value != null && !value.GetType().IsAssignableFrom(
                    typeof(MaskedTextBoxCell)))
                {
                    string s = "Cell type is not based upon the MaskedTextBoxCell.";
                    //CustomColumnMain.GetResourceManager().GetString("excNotMaskedTextBox");
                    throw new InvalidCastException(s);
                }

                base.CellTemplate = value;
            }
        }

        /// <summary>
        /// Indicates the Mask property that is used on the MaskedTextBox
        /// for entering new data into cells of this type.
        ///
        /// See the MaskedTextBox control documentation for more details.
        /// </summary>
        public virtual string Mask
        {
            get
            {
                return this.mask;
            }
            set
            {
                MaskedTextBoxCell mtbCell;
                DataGridViewCell dgvCell;
                int rowCount;

                if (this.mask != value)
                {
                    this.mask = value;

                    //
                    // First, update the value on the template cell.
                    //
                    mtbCell = (MaskedTextBoxCell)this.CellTemplate;
                    mtbCell.Mask = value;

                    //
                    // Now set it on all cells in other rows as well.
                    //
                    if (this.DataGridView != null && this.DataGridView.Rows != null)
                    {
                        rowCount = this.DataGridView.Rows.Count;
                        for (int x = 0; x < rowCount; x++)
                        {
                            dgvCell = this.DataGridView.Rows.SharedRow(x).Cells[x];
                            if (dgvCell is MaskedTextBoxCell)
                            {
                                mtbCell = (MaskedTextBoxCell)dgvCell;
                                mtbCell.Mask = value;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// By default, the MaskedTextBox uses the underscore (_) character
        /// to prompt for required characters.  This propertly lets you
        /// choose a different one.
        ///
        /// See the MaskedTextBox control documentation for more details.
        /// </summary>
        public virtual char PromptChar
        {
            get
            {
                return this.promptChar;
            }
            set
            {
                MaskedTextBoxCell mtbCell;
                DataGridViewCell dgvCell;
                int rowCount;

                if (this.promptChar != value)
                {
                    this.promptChar = value;

                    //
                    // First, update the value on the template cell.
                    //
                    mtbCell = (MaskedTextBoxCell)this.CellTemplate;
                    mtbCell.PromptChar = value;

                    //
                    // Now set it on all cells in other rows as well.
                    //
                    if (this.DataGridView != null && this.DataGridView.Rows != null)
                    {
                        rowCount = this.DataGridView.Rows.Count;
                        for (int x = 0; x < rowCount; x++)
                        {
                            dgvCell = this.DataGridView.Rows.SharedRow(x).Cells[x];
                            if (dgvCell is MaskedTextBoxCell)
                            {
                                mtbCell = (MaskedTextBoxCell)dgvCell;
                                mtbCell.PromptChar = value;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Indicates whether any unfilled characters in the mask should be
        /// be included as prompt characters when somebody asks for the text
        /// of the MaskedTextBox for a particular cell programmatically.
        ///
        /// See the MaskedTextBox control documentation for more details.
        /// </summary>
        public virtual bool IncludePrompt
        {
            get
            {
                return this.includePrompt;
            }
            set
            {
                MaskedTextBoxCell mtbc;
                DataGridViewCell dgvc;
                int rowCount;

                if (this.includePrompt != value)
                {
                    this.includePrompt = value;

                    //
                    // First, update the value on the template cell.
                    //
                    mtbc = (MaskedTextBoxCell)this.CellTemplate;
                    mtbc.IncludePrompt = TriBool(value);

                    //
                    // Now set it on all cells in other rows as well.
                    //
                    if (this.DataGridView != null && this.DataGridView.Rows != null)
                    {
                        rowCount = this.DataGridView.Rows.Count;
                        for (int x = 0; x < rowCount; x++)
                        {
                            dgvc = this.DataGridView.Rows.SharedRow(x).Cells[x];
                            if (dgvc is MaskedTextBoxCell)
                            {
                                mtbc = (MaskedTextBoxCell)dgvc;
                                mtbc.IncludePrompt = TriBool(value);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Controls whether or not literal (non-prompt) characters should
        /// be included in the output of the Text property for newly entered
        /// data in a cell of this type.
        ///
        /// See the MaskedTextBox control documentation for more details.
        /// </summary>
        public virtual bool IncludeLiterals
        {
            get
            {
                return this.includeLiterals;
            }
            set
            {
                MaskedTextBoxCell mtbCell;
                DataGridViewCell dgvCell;
                int rowCount;

                if (this.includeLiterals != value)
                {
                    this.includeLiterals = value;

                    //
                    // First, update the value on the template cell.
                    //
                    mtbCell = (MaskedTextBoxCell)this.CellTemplate;
                    mtbCell.IncludeLiterals = TriBool(value);

                    //
                    // Now set it on all cells in other rows as well.
                    //
                    if (this.DataGridView != null && this.DataGridView.Rows != null)
                    {

                        rowCount = this.DataGridView.Rows.Count;
                        for (int x = 0; x < rowCount; x++)
                        {
                            dgvCell = this.DataGridView.Rows.SharedRow(x).Cells[x];
                            if (dgvCell is MaskedTextBoxCell)
                            {
                                mtbCell = (MaskedTextBoxCell)dgvCell;
                                mtbCell.IncludeLiterals = TriBool(value);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Indicates the type against any data entered in the MaskedTextBox
        /// should be validated.  The MaskedTextBox control will attempt to
        /// instantiate this type and assign the value from the contents of
        /// the text box.  An error will occur if it fails to assign to this
        /// type.
        ///
        /// See the MaskedTextBox control documentation for more details.
        /// </summary>
        public virtual Type ValidatingType
        {
            get
            {
                return this.validatingType;
            }
            set
            {
                MaskedTextBoxCell mtbCell;
                DataGridViewCell dgvCell;
                int rowCount;

                if (this.validatingType != value)
                {
                    this.validatingType = value;

                    //
                    // First, update the value on the template cell.
                    //
                    mtbCell = (MaskedTextBoxCell)this.CellTemplate;
                    mtbCell.ValidatingType = value;

                    //
                    // Now set it on all cells in other rows as well.
                    //
                    if (this.DataGridView != null && this.DataGridView.Rows != null)
                    {
                        rowCount = this.DataGridView.Rows.Count;
                        for (int x = 0; x < rowCount; x++)
                        {
                            dgvCell = this.DataGridView.Rows.SharedRow(x).Cells[x];
                            if (dgvCell is MaskedTextBoxCell)
                            {
                                mtbCell = (MaskedTextBoxCell)dgvCell;
                                mtbCell.ValidatingType = value;
                            }
                        }
                    }
                }
            }
        }
    }
}
