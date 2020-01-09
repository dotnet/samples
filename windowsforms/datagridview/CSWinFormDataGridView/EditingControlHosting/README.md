# EditingControlHosting Sample

This sample demonstrates how to host a control in the current DataGridViewCell  
for editing.

## Remarks

There are six standard DataGridViewColumn types for use as follows:

DataGridViewTextBoxColumn
DataGridViewCheckedBoxColumn
DataGridViewComboBoxColumn
DataGridViewLinkColumn
DataGridViewButtonColumn
DataGridViewImageColumn

However, developers may want to use a different control for editing on the column,
e.g. MarkedTextBox, DateTimePicker etc. This feature can be achieved in two ways:

1. Create a custom DataGridViewColumn;

   For the details of how to do this, please refer to the CustomDataGridViewColumn
   sample.

1. Place the editing control on the current cell when editing begins, and hide
   the editing control when the editing completes.

   This sample demonstrates how to do this.

## Code Logic

1. Create an instance of the editing control, in this sample the editing control
   is MaskedTextBox.

1. Specify a mask for the MaskedTextBox and add the MaskedTextBox to the
   control collection of the DataGridView.

1. Hide the MaskedTextBox.

1. Handle the CellBeginEdit event to show the MaskedTextBox on the current
   editing cell.

1. Handle the CellEndEdit event to hide the MaskedTextBox when editing completes.

1. Handle the Scroll event to adjust the location of the MaskedTextBox as it is
   showing when scrolling the DataGridView.

1. Handle the EditingControlShowing event to pass the focus to the MaskedTextBox
   when begin editing with keystrokes.

## References

- [DataGridView Class](https://docs.microsoft.com/dotnet/api/system.windows.forms.datagridview)
