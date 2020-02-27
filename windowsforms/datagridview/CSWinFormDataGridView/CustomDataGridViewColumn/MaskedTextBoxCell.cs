/********************************* Module Header **********************************\
* Module Name:  MaskedTextBoxCell.cs
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
    class MaskedTextBoxCell : DataGridViewTextBoxCell
    {
        private string mask;
        private char promptChar;
        private DataGridViewTriState includePrompt;
        private DataGridViewTriState includeLiterals;
        private Type validatingType;

        /// <summary>
        /// Initializes a new instance of this class.  Fortunately, there's
        /// not much to do here except make sure that our base class is also
        /// initialized properly.
        /// </summary>
        public MaskedTextBoxCell()
            : base()
        {
            this.mask = "";
            this.promptChar = '_';
            this.includePrompt = DataGridViewTriState.NotSet;
            this.includeLiterals = DataGridViewTriState.NotSet;
            this.validatingType = typeof(string);
        }

        /// <summary>
        /// Whenever the user is to begin editing a cell of this type, the editing
        /// control must be created, which in this column type's case is a subclass
        /// of the MaskedTextBox control.
        ///
        /// This routine sets up all the properties and values on this control
        /// before the editing begins.
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="initialFormattedValue"></param>
        /// <param name="dataGridViewCellStyle"></param>
        public override void InitializeEditingControl(int rowIndex,
            object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            MaskedTextBoxEditingControl mtbEditingCtrl;
            MaskedTextBoxColumn mtbColumn;
            DataGridViewColumn dgvColumn;

            base.InitializeEditingControl(rowIndex, initialFormattedValue,
                                          dataGridViewCellStyle);

            mtbEditingCtrl = DataGridView.EditingControl as MaskedTextBoxEditingControl;

            //
            // Set up props that are specific to the MaskedTextBox
            //

            dgvColumn = this.OwningColumn;   // this.DataGridView.Columns[this.ColumnIndex];
            if (dgvColumn is MaskedTextBoxColumn)
            {
                mtbColumn = dgvColumn as MaskedTextBoxColumn;

                //
                // get the mask from this instance or the parent column.
                //
                if (string.IsNullOrEmpty(this.mask))
                {
                    mtbEditingCtrl.Mask = mtbColumn.Mask;
                }
                else
                {
                    mtbEditingCtrl.Mask = this.mask;
                }

                //
                // Prompt char.
                //
                mtbEditingCtrl.PromptChar = this.PromptChar;

                //
                // IncludePrompt
                //
                if (this.includePrompt == DataGridViewTriState.NotSet)
                {
                    //mtbEditingCtrl.IncludePrompt = mtbcol.IncludePrompt;
                }
                else
                {
                    //mtbEditingCtrl.IncludePrompt = BoolFromTri(this.includePrompt);
                }

                //
                // IncludeLiterals
                //
                if (this.includeLiterals == DataGridViewTriState.NotSet)
                {
                    //mtbEditingCtrl.IncludeLiterals = mtbcol.IncludeLiterals;
                }
                else
                {
                    //mtbEditingCtrl.IncludeLiterals = BoolFromTri(this.includeLiterals);
                }

                //
                // Finally, the validating type ...
                //
                if (this.ValidatingType == null)
                {
                    mtbEditingCtrl.ValidatingType = mtbColumn.ValidatingType;
                }
                else
                {
                    mtbEditingCtrl.ValidatingType = this.ValidatingType;
                }

                mtbEditingCtrl.Text = (string)this.Value;
            }
        }

        /// <summary>
        /// Returns the type of the control that will be used for editing
        /// cells of this type.  This control must be a valid Windows Forms
        /// control and must implement IDataGridViewEditingControl.
        /// </summary>
        public override Type EditType
        {
            get
            {
                return typeof(MaskedTextBoxEditingControl);
            }
        }

        /// <summary>
        /// A string value containing the Mask against input for cells of
        /// this type will be verified.
        /// </summary>
        public virtual string Mask
        {
            get
            {
                return this.mask;
            }
            set
            {
                this.mask = value;
            }
        }

        /// <summary>
        /// The character to use for prompting for new input.
        /// </summary>
        public virtual char PromptChar
        {
            get
            {
                return this.promptChar;
            }
            set
            {
                this.promptChar = value;
            }
        }

        /// <summary>
        /// A boolean indicating whether to include prompt characters in
        /// the Text property's value.
        /// </summary>
        public virtual DataGridViewTriState IncludePrompt
        {
            get
            {
                return this.includePrompt;
            }
            set
            {
                this.includePrompt = value;
            }
        }

        /// <summary>
        /// A boolean value indicating whether to include literal characters
        /// in the Text property's output value.
        /// </summary>
        public virtual DataGridViewTriState IncludeLiterals
        {
            get
            {
                return this.includeLiterals;
            }
            set
            {
                this.includeLiterals = value;
            }
        }

        /// <summary>
        /// A Type object for the validating type.
        /// </summary>
        public virtual Type ValidatingType
        {
            get
            {
                return this.validatingType;
            }
            set
            {
                this.validatingType = value;
            }
        }

        /// <summary>
        /// Quick routine to convert from DataGridViewTriState to boolean.
        /// True goes to true while False and NotSet go to false.
        /// </summary>
        /// <param name="tri"></param>
        /// <returns></returns>
        protected static bool BoolFromTri(DataGridViewTriState tri)
        {
            return (tri == DataGridViewTriState.True) ? true : false;
        }
    }
}
