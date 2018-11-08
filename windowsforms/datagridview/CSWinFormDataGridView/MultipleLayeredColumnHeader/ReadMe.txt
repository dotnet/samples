================================================================================
       WINDOWS FORMS APPLICATION : CSWinFormDataGridView Project Overview
       
                   MultipleLayeredColumnHeader Sample
                        
===============================================================================

/////////////////////////////////////////////////////////////////////////////
Use:

This sample demonstrates how to display multiple layer column headers on the 
DataGridView contorl.


-----------------------------------------------------------------
      |   January      |   February      |     March      |
      |  Win  | Loss   |  Win   | Loss   |  Win   | Loss  |
-----------------------------------------------------------------
Team1 |       |        |        |        |        |       |
-----------------------------------------------------------------
Team2 |       |        |        |        |        |       |
-----------------------------------------------------------------
TeamN |       |        |        |        |        |       |
-----------------------------------------------------------------


/////////////////////////////////////////////////////////////////////////////
Code Logic:

1.  Enable resizing on the column headers by setting the 
    ColumnHeadersHeightSizeMode property as follows:

    this.dataGridView1.ColumnHeadersHeightSizeMode =
         DataGridViewColumnHeadersHeightSizeMode.EnableResizing;

2.  Adjust the height for the column headers to make it wide enough for two 
    layers;

    this.dataGridView1.ColumnHeadersHeight =
         this.dataGridView1.ColumnHeadersHeight * 2;
                        
                        
3.  Adjust the text alignment on the column headers to make the text display 
    at the center of the bottom;
    
    this.dataGridView1.ColumnHeadersDefaultCellStyle.Alignment =
         DataGridViewContentAlignment.BottomCenter;
    
4.  Handle the DataGridView.CellPainting event to draw text for each header 
    cell;

5.  Handle the DataGridView.Paint event to draw "merged" header cells;


/////////////////////////////////////////////////////////////////////////////
References:

1. DataGridView Class
   http://msdn.microsoft.com/en-us/library/system.windows.forms.datagridview.aspx
   
2. Windows Forms FAQs
   http://windowsclient.net/blogs/faqs/archive/tags/Custom+Designers/default.aspx


/////////////////////////////////////////////////////////////////////////////