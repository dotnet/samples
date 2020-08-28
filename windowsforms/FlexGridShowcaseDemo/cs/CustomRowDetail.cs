using C1.Win.FlexGrid;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace FlexGridShowcaseDemo
{
    /// <summary>
    /// Custom row detail class which shows label with notes about employee.
    /// </summary>
    public class CustomRowDetail :  Label, IC1FlexGridRowDetail
    {
        /// <summary>
        /// Used to setup control before showing of it.
        /// </summary>
        /// <param name="parentGrid">FlexGrid which displays detail control.</param>
        /// <param name="rowIndex">Index of parent detail row.</param>
        void IC1FlexGridRowDetail.Setup(C1FlexGrid parentGrid, int rowIndex)
        {
            using (var bindingSource = new BindingSource(parentGrid.DataSource as DataSet, "Products"))
            {
                bindingSource.Position = parentGrid.Rows[rowIndex].DataIndex;
                var row = bindingSource.Current as DataRowView;

                // Formatting text
                var columnTitles = (from s in row.Row.Table.Columns.Cast<DataColumn>() select s)
                    .Where(x => x.ColumnName != "Name")
                    .Select(x => x.ColumnName);

                var details = columnTitles
                    .Select(x => string.Format("{0}: {1}", x, row[x]));

                Text = string.Join(Environment.NewLine, details);
            }
        }

        /// <summary>
        /// Used to update size of the control.
        /// </summary>
        /// <param name="parentGrid">FlexGrid which displays detail control.</param>
        /// <param name="rowIndex">Index of parent detail row.</param>
        /// <param name="proposedSize">The proposed size for the detail control.</param>
        void IC1FlexGridRowDetail.UpdateSize(C1FlexGrid parentGrid, int rowIndex, Size proposedSize)
        {
            var scrollableRectangleSize = parentGrid.ScrollableRectangle.Size;
            var labelSize = TextRenderer.MeasureText(Text, Font, scrollableRectangleSize, TextFormatFlags.WordBreak);
            labelSize.Width = Math.Max(labelSize.Width, scrollableRectangleSize.Width);

            Size = labelSize;
        }

        /// <summary>
        /// Used to release resources of the control before the removing of it.
        /// </summary>
        /// <param name="parentGrid">FlexGrid which displays detail control.</param>
        /// <param name="rowIndex">Index of parent detail row.</param>
        void IC1FlexGridRowDetail.Removing(C1FlexGrid parentGrid, int rowIndex)
        {
            // No resources to release
        }
    }
}
