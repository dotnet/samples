/************************************* Module Header **************************************\
* Module Name:  MaskedTextBoxColumn.cs
* Project:      CSWinFormDataGridView
* Copyright (c) Microsoft Corporation.
*
* This sample demonstrates the use of custom column definitions within the Windows Forms
* DataGridView control.
*
* The Employee ID, SSN, State and Zip Code columns use MaskedTextBox controls for format
* and validate their input.
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

namespace CSWinFormDataGridView.CustomDataGridViewColumn
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            DataGridViewTextBoxColumn dgvTextBoxColumn;
            MaskedTextBoxColumn mtbColumn;

            //
            // Employee name.
            //
            dgvTextBoxColumn = new DataGridViewTextBoxColumn();
            dgvTextBoxColumn.HeaderText = "Name";
            dgvTextBoxColumn.Width = 120;
            this.employeesDataGridView.Columns.Add(dgvTextBoxColumn);

            //
            // Employee ID -- this will be of the format:
            // [A-Z][0-9][0-9][0-9][0-9][0-9]
            //
            // this is well suited to using a MaskedTextBox column.
            //
            mtbColumn = new MaskedTextBoxColumn();
            mtbColumn.HeaderText = "Employee ID";
            mtbColumn.Mask = "L00000";
            mtbColumn.Width = 75;
            this.employeesDataGridView.Columns.Add(mtbColumn);

            //
            // [American] Social Security number, of the format:
            // ###-##-####
            //
            mtbColumn = new MaskedTextBoxColumn();
            mtbColumn.HeaderText = "SSN";
            mtbColumn.Mask = "000-00-0000";
            mtbColumn.Width = 75;
            this.employeesDataGridView.Columns.Add(mtbColumn);

            //
            // Address
            //
            dgvTextBoxColumn = new DataGridViewTextBoxColumn();
            dgvTextBoxColumn.HeaderText = "Address";
            dgvTextBoxColumn.Width = 150;
            this.employeesDataGridView.Columns.Add(dgvTextBoxColumn);

            //
            // City
            //
            dgvTextBoxColumn = new DataGridViewTextBoxColumn();
            dgvTextBoxColumn.HeaderText = "City";
            dgvTextBoxColumn.Width = 75;
            this.employeesDataGridView.Columns.Add(dgvTextBoxColumn);

            //
            // State
            //
            mtbColumn = new MaskedTextBoxColumn();
            mtbColumn.HeaderText = "State";
            mtbColumn.Mask = "LL";
            mtbColumn.Width = 40;
            this.employeesDataGridView.Columns.Add(mtbColumn);

            //
            // Zip Code #####-#### (+4 optional)
            //
            mtbColumn = new MaskedTextBoxColumn();
            mtbColumn.HeaderText = "Zip Code";
            mtbColumn.Mask = "00000-0000";
            mtbColumn.Width = 75;
            mtbColumn.ValidatingType = typeof(ZipCode);
            this.employeesDataGridView.Columns.Add(mtbColumn);

            //
            // Department Code
            //
            dgvTextBoxColumn = new DataGridViewTextBoxColumn();
            dgvTextBoxColumn.HeaderText = "Department";
            dgvTextBoxColumn.ValueType = typeof(int);
            dgvTextBoxColumn.Width = 75;
            this.employeesDataGridView.Columns.Add(dgvTextBoxColumn);
        }
    }

    // Type that represents a US Zipcode to demonstrate
    // the ValidatingType feature on the MaskedTextBox.
    #region ZipCode Class

    public class ZipCode
    {
        private int zipCode;
        private int plusFour;

        public ZipCode()
        {
            this.zipCode = 0;
            this.plusFour = 0;
        }

        public ZipCode(string in_zipCode)
        {
            parseFromString(in_zipCode, out zipCode, out plusFour);
        }

        public ZipCode(int in_ivalue)
        {
            this.zipCode = in_ivalue;
            this.plusFour = 0;
        }

        public static implicit operator ZipCode(string s)
        {
            return new ZipCode(s);
        }

        public static implicit operator ZipCode(int i)
        {
            return new ZipCode(i);
        }

        protected static void parseFromString
        (
            string in_string,
            out int out_zipCode,
            out int out_plusFour
        )
        {
            int zc = 0, pf = 0;
            char[] charray;
            int x = 0;

            if (in_string == null || in_string.Equals(""))
            {
                throw new ArgumentException("Invalid String");
            }

            charray = in_string.Trim().ToCharArray();

            for (x = 0; x < 5; x++)
            {
                if (!Char.IsDigit(charray[x]))
                {
                    throw new ArgumentException("Invalid String");
                }

                zc = zc * 10 + numfromchar(charray[x]);
            }

            while (x < charray.Length && !Char.IsDigit(charray[x]))
            {
                x++;
            }

            if (x < charray.Length)
            {
                for (int y = x; y < 4; y++)
                {
                    if (!Char.IsDigit(charray[y]))
                    {
                        throw new ArgumentException("Invalid ZipCode String");
                    }

                    pf = pf * 10 + numfromchar(charray[y]);
                }
            }

            out_zipCode = zc;
            out_plusFour = pf;
        }

        private static int numfromchar(char c)
        {
            switch (c)
            {
                case '0':
                    return 0;
                case '1':
                    return 1;
                case '2':
                    return 2;
                case '3':
                    return 3;
                case '4':
                    return 4;
                case '5':
                    return 5;
                case '6':
                    return 6;
                case '7':
                    return 7;
                case '8':
                    return 8;
                case '9':
                    return 9;

                default:
                    throw new ArgumentException("invalid digit");
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(10);

            sb.Append(zipCode.ToString("00000"));
            sb.Append("-");
            sb.Append(plusFour.ToString("0000"));

            return sb.ToString();
        }
    }

    #endregion

}
