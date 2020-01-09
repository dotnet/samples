# CustomDataGridViewColumn Sample

This sample demonstrates how to create a custom DataGridView column.

## Remarks

There are six standard DataGridViewColumn types for use as follows:

- DataGridViewTextBoxColumn
- DataGridViewCheckedBoxColumn
- DataGridViewComboBoxColumn
- DataGridViewLinkColumn
- DataGridViewButtonColumn
- DataGridViewImageColumn

However, developers may want to use a different control for editing on the column,
e.g. MarkedTextBox, DateTimePicker etc. This feature can be achieved in two ways:

1. Create a custom DataGridViewColumn;

   The code in this CustomDataGridViewColumn sample demonstrates how to do this.

2. Place the editing control on the current cell when editing begins, and hide
   the editing control when the editing completes. For the details of this
   approach, please refer to the EditingControlHosting sample.

## Creation

1. Create a MaskedTextBoxEditingControl class derive from MaskedTextBox class
   and IDataGridViewEditingControl class, see the code in the
   MaskedTextBoxEditingControl.cs file for the implementation details.

2. Create a MaskedTextBoxCell class derive from DataGridViewTextBoxCell class,
   see the code in the MaskedTextBoxCell.cs file for the implementation details.

3. Create a MaskedTextBoxColumn class derive from DataGridViewColumn class,
   see the code in the MaskedTextBoxColumn.cs file for the implementation details.

4. Build the program.
