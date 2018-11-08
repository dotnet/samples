================================================================================
       WINDOWS FORMS APPLICATION : CSWinFormDataGridView Project Overview
       
                   JustInTimeDataLoading Sample
                        
===============================================================================

/////////////////////////////////////////////////////////////////////////////
Use:

 This sample demonstrates how to use virtual mode in the DataGridView control 
 with a data cache that loads data from a server only when it is needed. 
 This kind of data loading is called "Just-in-time data loading". 


/////////////////////////////////////////////////////////////////////////////
Remark:

 If you are working with a very large table in a remote database, for example, 
 you might want to avoid startup delays by retrieving only the data that is 
 necessary for display and retrieving additional data only when the user scrolls 
 new rows into view. If the client computers running your application have a 
 limited amount of memory available for storing data, you might also want to 
 discard unused data when retrieving new values from the database.


/////////////////////////////////////////////////////////////////////////////
Code Logic:

1.  Enable VirtualMode on the DataGridView control by setting the VirtualMode
    property to true:
    
    this.dataGridView1.VirtualMode = true;
                        
2.  Add columns to the DataGridView according to the data in the database;

3.  Retrieve the row count of the data in the database and set the RowCount 
    property for the DataGridView;
    
4.  Handle the CellValueNeeded event to retrieve the requested cell value 
    from the data store or the Customer object currently in edit. 


/////////////////////////////////////////////////////////////////////////////
References:

1. Implementing Virtual Mode with Just-In-Time Data Loading in the Windows 
   Forms DataGridView Control
   http://msdn.microsoft.com/en-us/library/ms171624.aspx
   
2. Windows Forms FAQs
   http://windowsclient.net/blogs/faqs/archive/tags/Custom+Designers/default.aspx


/////////////////////////////////////////////////////////////////////////////