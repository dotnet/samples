# MultipleLayeredColumnHeader Sample

This sample demonstrates how to display multiple layer column headers on the
DataGridView contorl.

```
------------------------------------------------------------
|      |   January      |   February      |     March      |
|      |  Win  | Loss   |  Win   | Loss   |  Win   | Loss  |
------------------------------------------------------------
|Team1 |       |        |        |        |        |       |
------------------------------------------------------------
|Team2 |       |        |        |        |        |       |
------------------------------------------------------------
|TeamN |       |        |        |        |        |       |
------------------------------------------------------------
```

## Code Logic

1. Enable resizing on the column headers by setting the ColumnHeadersHeightSizeMode property as follows:

    ```CSharp
        this.dataGridView1.ColumnHeadersHeightSizeMode =
            DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
    ```

1. Adjust the height for the column headers to make it wide enough for two layers:

    ```CSharp
        this.dataGridView1.ColumnHeadersHeight =
            this.dataGridView1.ColumnHeadersHeight * 2;
    ```

1. Adjust the text alignment on the column headers to make the text display at the center of the bottom:

    ```CSharp
        this.dataGridView1.ColumnHeadersDefaultCellStyle.Alignment =
            DataGridViewContentAlignment.BottomCenter;
    ```

1. Handle the DataGridView.CellPainting event to draw text for each header cell.

1. Handle the DataGridView.Paint event to draw "merged" header cells.

## References

- [DataGridView Class](https://docs.microsoft.com/dotnet/api/system.windows.forms.datagridview)
