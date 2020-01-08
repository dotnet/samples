# JustInTimeDataLoading Sample

This sample demonstrates how to use virtual mode in the DataGridView control
with a data cache that loads data from a server only when it is needed.
This kind of data loading is called "Just-in-time data loading".

## Remarks

If you are working with a very large table in a remote database, for example,
you might want to avoid startup delays by retrieving only the data that is
necessary for display and retrieving additional data only when the user scrolls
new rows into view. If the client computers running your application have a
limited amount of memory available for storing data, you might also want to
discard unused data when retrieving new values from the database.

## Code Logic

1. Enable VirtualMode on the DataGridView control by setting the VirtualMode property to true:

    ```CSharp
    this.dataGridView1.VirtualMode = true;
    ```

1. Add columns to the DataGridView according to the data in the database.

1. Retrieve the row count of the data in the database and set the RowCount property for the DataGridView.

1. Handle the CellValueNeeded event to retrieve the requested cell value from the data store or the Customer object currently in edit.

## References

- [Implementing Virtual Mode with Just-In-Time Data Loading in the Windows Forms DataGridView Control](https://docs.microsoft.com/dotnet/framework/winforms/controls/implementing-virtual-mode-jit-data-loading-in-the-datagrid)
