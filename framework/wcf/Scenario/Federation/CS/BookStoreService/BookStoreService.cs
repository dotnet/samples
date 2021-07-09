//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.IO;
using System.Security.Principal;
using System.ServiceModel;

namespace Microsoft.Samples.Federation
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
	public class BookStoreService : IBrowseBooks, IBuyBook
    {
        #region BookStoreService Constructor
        /// <summary>
        /// Sets up the BookStoreService by loading relevant Application Settings
        /// </summary>
        public BookStoreService()
        {
        }

        #endregion

        #region BrowseBooks() Implementation
        /// <summary>
        /// browseBooks() service call Implementation
        /// </summary>
        /// <returns>List of books available for purchase in the bookstore</returns>
        public List<string> BrowseBooks()
        {
            // Create an empty list of strings.
            List<string> books = new List<string>();
            try
            {
                // Create a StreamReader over the text file specified in app.config
                using (StreamReader myStreamReader = new StreamReader(ServiceConstants.BookDB))
                {
                    string line = "";
                    // For each line in the text file...
                    while ((line = myStreamReader.ReadLine()) != null)
                    {
                        // ...split the text from the text file...
                        string[] splitEntry = line.Split('#');
                        // ...format a string to return...
                        string formattedEntry = String.Format("{0}.  {1},  {2},  ${3}",
                                                                splitEntry[0], // Book ID 
                                                                splitEntry[1], // Book Name
                                                                splitEntry[2], // Author
                                                                splitEntry[3] // Price
                                                              );
                        // ...and add it to the list 
                        books.Add(formattedEntry);
                    }
                    // Once we've finished reading the file, return the list of strings
                    return books;
                }
            }
            catch (Exception e)
            {
                throw new Exception(String.Format("BookStoreService: Error while loading books from DB ", e));
            }
        }
		#endregion

		#region BuyBook() Implementation

        /// <summary>
        /// This function extracts a Name claim from the provided ServiceSecurityContext and 
        /// returns the associated resource value.
        /// </summary>
        /// <param name="securityContext">The ServiceSecurityContext in which the Name claim should be found</param>
        /// <returns>The resource value associated </returns>
        static string GetNameIdentity(ServiceSecurityContext securityContext)
        {
            // Iterate through each of the claimsets in the AuthorizationContext.
            foreach (ClaimSet claimSet in securityContext.AuthorizationContext.ClaimSets)
            {
                // Find all the Name claims
                IEnumerable<Claim> nameClaims = claimSet.FindClaims(ClaimTypes.Name, Rights.PossessProperty);
                if (nameClaims != null)
                {
                    // Get the first claim 
                    IEnumerator<Claim> enumerator = nameClaims.GetEnumerator();
                    if (enumerator.MoveNext())
                    {
                        // return the resource value.
                        return enumerator.Current.Resource.ToString();
                    }
                }
            }

            // If there are no Name claims in the AuthorizationContext, return the Name of the Anonymous Windows Identity.
            return WindowsIdentity.GetAnonymous().Name;
        }

        public string BuyBook(string emailAddress, string shipAddress)
		{
            // get the book id from the headers
            string bookName = OperationContext.Current.IncomingMessageHeaders.GetHeader<string>(Constants.BookNameHeaderName, Constants.BookNameHeaderNamespace);
            if (bookName == null)
            {
                throw new FaultException<string>("The name of the book to be purchased was not specified");
            }
            // Get the callers name
            string caller = GetNameIdentity(ServiceSecurityContext.Current);
            return String.Format("{0}, the purchase of book {1} has been approved. The details of shipping date and confirmation receipt will be mailed to {2} shortly", caller,
                bookName, emailAddress);
		}
		#endregion
	}
}

