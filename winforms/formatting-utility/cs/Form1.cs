using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Formatter
{
    public partial class Form1 : Form
    {
        private ResourceManager rm;

        private ToolStripStatusLabel label;

        private string decimalSeparator;
        private string amDesignator, pmDesignator, aDesignator, pDesignator;
        private string pattern;

        // Flags to indicate presence of error information in status bar
        bool valueInfo;
        bool formatInfo;

        private string[] numberFormats = { "C", "D", "E", "e", "F", "G", "N", "P", "R", "X", "x" };
        private const int DEFAULTSELECTION = 5;
        private string[] dateFormats = { "g", "d", "D", "f", "F", "g", "G", "M", "O", "R", "s",
                                       "t", "T", "u", "U", "Y" };

        public Form1()
        {
            InitializeComponent();
            rm = new ResourceManager("Formatter.Resources", this.GetType().Assembly);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Disable OK button.
            OKButton.Enabled = false;

            // Add label to status bar.
            label = new ToolStripStatusLabel();
            ToolStripItem[] items = { label };
            this.StatusBar.Items.AddRange(items);

            // Get localized strings for user interface.
            this.Text = rm.GetString("WindowCaption");
            this.ValueLabel.Text = rm.GetString("ValueLabel");
            this.FormatLabel.Text = rm.GetString("FormatLabel");
            this.ResultLabel.Text = rm.GetString("ResultLabel");
            this.CulturesLabel.Text = rm.GetString("CultureLabel");
            this.NumberBox.Text = rm.GetString("NumberBoxText");
            this.DateBox.Text = rm.GetString("DateBoxText");
            this.OKButton.Text = rm.GetString("OKButtonText");

            // Populate CultureNames list box with culture names
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            // Define a string list so that we can sort and modify the names.
            var names = new List<string>();
            var currentIndex = 0;                    // Index of the current culture.

            foreach (var culture in cultures)
                names.Add(culture.Name);

            names.Sort();
            // Change the name of the invariant culture so it is human readable.
            names[0] = rm.GetString("InvariantCultureName");
            // Add the culture names to the list box.
            this.CultureNames.Items.AddRange(names.ToArray());

            // Make the current culture the selected culture.
            for (int ctr = 0; ctr < names.Count; ctr++)
            {
                if (names[ctr] == CultureInfo.CurrentCulture.Name)
                {
                    currentIndex = ctr;
                    break;
                }
            }
            this.CultureNames.SelectedIndex = currentIndex;

            // Get decimal separator.
            decimalSeparator = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;

            // Get am, pm designators.
            amDesignator = DateTimeFormatInfo.CurrentInfo.AMDesignator;
            if (amDesignator.Length >= 1)
                aDesignator = amDesignator.Substring(0, 1);
            else
                aDesignator = String.Empty;

            pmDesignator = DateTimeFormatInfo.CurrentInfo.PMDesignator;
            if (pmDesignator.Length >= 1)
                pDesignator = pmDesignator.Substring(0, 1);
            else
                pDesignator = String.Empty;

            // For regex pattern for date and time components.
            pattern = @$"^\s*\S+\s+\S+\s+\S+(\s+\S+)?(?<!{amDesignator}|{aDesignator}|{pmDesignator}|{pDesignator})\s*$";

            // Select NumberBox for numeric string and populate combo box.
            this.NumberBox.Checked = true;
        }

        private void NumberBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.NumberBox.Checked)
            {
                this.Result.Text = String.Empty;

                this.FormatStrings.Items.Clear();
                this.FormatStrings.Items.AddRange(numberFormats);
                this.FormatStrings.SelectedIndex = DEFAULTSELECTION;
            }
        }

        private void OKButton_Click(object sender, EventArgs e)
        {

            label.Text = "";
            this.Result.Text = String.Empty;

            // Get name of the current culture.
            CultureInfo culture = null;
            string cultureName = (string)this.CultureNames.Items[this.CultureNames.SelectedIndex];
            // If the selected culture is the invariant culture, change its name.
            if (cultureName == rm.GetString("InvariantCultureName"))
                cultureName = String.Empty;
            culture = CultureInfo.CreateSpecificCulture(cultureName);

            // Parse string as date.
            if (this.DateBox.Checked)
            {
                DateTime dat = DateTime.MinValue;
                DateTimeOffset dto = DateTimeOffset.MinValue;
                long ticks;
                bool hasOffset = false;

                // Is the date a number expressed in ticks?
                if (Int64.TryParse(this.Value.Text, out ticks))
                {
                    dat = new DateTime(ticks);
                }
                else
                {
                    // Does the date have three components (date, time offset), or fewer than 3?
                    if (Regex.IsMatch(this.Value.Text, pattern, RegexOptions.IgnoreCase))
                    {
                        if (DateTimeOffset.TryParse(this.Value.Text, out dto))
                        {
                            hasOffset = true;
                        }
                        else
                        {
                            label.Text = rm.GetString("MSG_InvalidDTO");
                            valueInfo = true;
                            return;
                        }
                    }
                    else
                    {
                        // The string is to be interpeted as a DateTime, not a DateTimeOffset.
                        if (DateTime.TryParse(this.Value.Text, out dat))
                        {
                            hasOffset = false;
                        }
                        else
                        {
                            label.Text = rm.GetString("MSG_InvalidDate");
                            valueInfo = true;
                            return;
                        }
                    }
                }
                // Format date value.
                this.Result.Text = (hasOffset ? dto : dat).ToString(this.FormatStrings.Text, culture);
            }
            else
            {
                // Handle formatting of a number.
                long intToFormat;
                BigInteger bigintToFormat = BigInteger.Zero;
                double floatToFormat;

                // Format a floating point value.
                if (Value.Text.Contains(decimalSeparator) || Value.Text.ToUpper(CultureInfo.InvariantCulture).Contains("E"))
                {
                    try
                    {
                        if (!Double.TryParse(Value.Text, out floatToFormat))
                            label.Text = rm.GetString("MSG_InvalidFloat");
                        else
                            this.Result.Text = floatToFormat.ToString(this.FormatStrings.Text, culture);
                    }
                    catch (FormatException)
                    {
                        label.Text = rm.GetString("MSG_InvalidFormat");
                        this.formatInfo = true;
                    }
                }
                else
                {
                    // Handle formatting an integer.
                    //
                    // Determine whether value is out of range of an Int64
                    if (!BigInteger.TryParse(Value.Text, out bigintToFormat))
                    {
                        label.Text = rm.GetString("MSG_InvalidInteger");
                    }
                    else
                    {
                        // Format an Int64
                        if (bigintToFormat >= Int64.MinValue && bigintToFormat <= Int64.MaxValue)
                        {
                            intToFormat = (long)bigintToFormat;
                            try
                            {
                                this.Result.Text = intToFormat.ToString(this.FormatStrings.Text, culture);
                            }
                            catch (FormatException)
                            {
                                label.Text = rm.GetString("MSG_InvalidFormat");
                                this.formatInfo = true;
                            }
                        }
                        else
                        {
                            // Format a BigInteger
                            try
                            {
                                this.Result.Text = bigintToFormat.ToString(this.FormatStrings.Text, culture);
                            }
                            catch (FormatException)
                            {
                                label.Text = rm.GetString("MSG_InvalidFormat");
                                this.formatInfo = true;
                            }
                        }
                    }
                }
            }
        }

        private void DateBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.DateBox.Checked)
            {
                this.Result.Text = String.Empty;

                this.FormatStrings.Items.Clear();
                this.FormatStrings.Items.AddRange(dateFormats);
                this.FormatStrings.SelectedIndex = DEFAULTSELECTION;
            }
        }

        private void Value_TextChanged(object sender, EventArgs e)
        {
            this.Result.Text = String.Empty;

            if (valueInfo)
            {
                label.Text = String.Empty;
                valueInfo = false;
            }
            OKButton.Enabled = !string.IsNullOrEmpty(Value.Text);
        }

        private void FormatStrings_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Result.Text = String.Empty;
            if (formatInfo)
            {
                label.Text = String.Empty;
                formatInfo = false;
            }
        }

        private void CultureNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Result.Text = String.Empty;
        }
    }
}
