//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
using System;

using System.Globalization;

using System.ServiceModel;
using System.ServiceModel.Channels;

using System.Windows.Forms;

namespace Microsoft.Samples.Federation
{
    public partial class BookStoreClientForm : Form
    {
        #region Form Constructor
        /// <summary>
        /// Sets up the GUI 
        /// </summary>
        public BookStoreClientForm()
        {
            InitializeComponent();
        }
        #endregion

        #region Browse Button Event Handler
        /// <summary>
        ///  Handles the operation of browsing the available books
        /// </summary>
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            // refresh list view
            lstBooks.Items.Clear();

            // create proxy object
            BrowseBooksClient client = new BrowseBooksClient();

            try
            {

                // call the browseBooks() method
                string[] books = client.BrowseBooks();

                // load results in list view
                for (int i = 0; i < books.Length; i++)
                {
                    lstBooks.Items.Add(books[i]);
                }
                MessageBox.Show("Books Loaded for Browsing", "Bookstore Client");

                if (!btnBuy.Enabled)
                {
                    btnBuy.Enabled = true;
                }

                client.Close();
            }
            catch (EndpointNotFoundException)
            {
                MessageBox.Show("Error while loading books for browsing. Make sure the BookStore Service, BookStore STS, and HomeRealm STS have been started", "Bookstore Client");
            }
            catch (CommunicationException ex)
            {
                client.Abort();
                MessageBox.Show(String.Format("Communication error while loading books for browsing: {0}", ex), "Bookstore Client");
            }
            catch (TimeoutException ex)
            {
                client.Abort();
                MessageBox.Show(String.Format("Timeout error while loading books for browsing: {0}", ex), "Bookstore Client");
            }
            catch (Exception ex)
            {
                client.Abort();
                MessageBox.Show(String.Format("Unexpected error while loading books for browsing: {0}", ex), "Bookstore Client");
            }
        }
        #endregion

        #region Buy Button Event Handler
        /// <summary>
        /// Handles the operation of buying a selected book
        /// </summary>
        private void btnBuy_Click(object sender, EventArgs e)
        {
            // check if any book is selected for purchase
            if (lstBooks.SelectedItem == null)
            {
                MessageBox.Show("No book selected for purchase", "BookStore Client");
            }
            else
            {
                // get the selected book ID
                string selectedBookItem = lstBooks.SelectedItem.ToString();
                int startPos = selectedBookItem.IndexOf('.') + 1;
                int endPos = selectedBookItem.IndexOf(',');
                string bookName = selectedBookItem.Substring(startPos, endPos - startPos);
                bookName = bookName.Trim();

                BuyBookClient myBuyBookClient = new BuyBookClient();

                try
                {


                    // Add the book name as a "resource" header to the endpoint address for the service
                    EndpointAddressBuilder myEndpointAddressBuilder = new EndpointAddressBuilder(myBuyBookClient.Endpoint.Address);
                    myEndpointAddressBuilder.Headers.Add(AddressHeader.CreateAddressHeader(Constants.BookNameHeaderName, Constants.BookNameHeaderNamespace, bookName));
                    myBuyBookClient.Endpoint.Address = myEndpointAddressBuilder.ToEndpointAddress();

                    // Send the request to the service. This will result in the following steps;
                    // 1. Request a security token from HomeRealmSTS authenticating using Windows credentials
                    // 2. Request a security token from BookStoreSTS authenticating using token from 1.
                    // 3. Send the BuyBook request to the BookStoreService authenticating using token from 2.
                    string response = myBuyBookClient.BuyBook("someone@microsoft.com", "One Microsoft Way, Redmond, WA 98052");
                    MessageBox.Show(response, "BookStore Client");

                    myBuyBookClient.Close();
                }
                catch (Exception ex)
                {
                    myBuyBookClient.Abort();

                    // see if a fault has been sent back
                    FaultException inner = GetInnerException(ex) as FaultException;
                    if (inner != null)
                    {
                        MessageFault fault = inner.CreateMessageFault();
                        MessageBox.Show(String.Format("The server sent back a fault: {0}", fault.Reason.GetMatchingTranslation(CultureInfo.CurrentCulture).Text));
                    }
                    else
                    {
                        MessageBox.Show(String.Format("Exception while trying to purchase the selected book: {0}", ex), "BookStore Client");
                    }
                }
            }
        }
        #endregion

        #region Helper method to get inner exception
        private static Exception GetInnerException(Exception ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException("ex");
            }

            Exception innerEx = ex;
            while (innerEx.InnerException != null)
            {
                innerEx = innerEx.InnerException;
            }

            return innerEx;
        }
        #endregion
    }
}
