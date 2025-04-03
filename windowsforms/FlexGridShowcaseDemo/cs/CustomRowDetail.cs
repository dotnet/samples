using System.Data;
using C1.Win.FlexGrid;

namespace FlexGridShowcaseDemo;

/// <summary>
/// Custom row detail class which shows label with notes about employee.
/// </summary>
public class CustomRowDetail : Label, IC1FlexGridRowDetail
{
    /// <summary>
    /// Used to setup control before showing of it.
    /// </summary>
    /// <param name="parentGrid">FlexGrid which displays detail control.</param>
    /// <param name="rowIndex">Index of parent detail row.</param>
    void IC1FlexGridRowDetail.Setup(C1FlexGrid parentGrid, int rowIndex)
    {
        if (parentGrid?.DataSource is not DataSet dataSet)
        {
            return;
        }

        DataTable dataDataTable = dataSet.Tables["Data"];
        DataRow dataRow = dataDataTable.Rows[parentGrid.Rows[rowIndex].DataIndex];

        DataRow[] childRows = dataRow.GetChildRows("Products");
        if (childRows?.Length == 0)
        {
            return;
        }

        DataRow detailRow = childRows[0];
        if (detailRow is null)
        {
            return;
        }

        // Formatting text
        IEnumerable<string> details = (from s in detailRow.Table.Columns.Cast<DataColumn>() select s)
                .Where(x => x.ColumnName != "Name")
                .Select(x => $"{x.ColumnName}: {detailRow[x.ColumnName]}");

        Text = string.Join(Environment.NewLine, details);
    }

    /// <summary>
    /// Used to update size of the control.
    /// </summary>
    /// <param name="parentGrid">FlexGrid which displays detail control.</param>
    /// <param name="rowIndex">Index of parent detail row.</param>
    /// <param name="proposedSize">The proposed size for the detail control.</param>
    void IC1FlexGridRowDetail.UpdateSize(C1FlexGrid parentGrid, int rowIndex, Size proposedSize)
    {
        Size scrollableRectangleSize = parentGrid.ScrollableRectangle.Size;
        Size labelSize = TextRenderer.MeasureText(Text, Font, scrollableRectangleSize, TextFormatFlags.WordBreak);
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
