//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Threading;

namespace Microsoft.Samples.AspNetCachingIntegration
{
    class Program
    {
        static readonly DataContractSerializer customerSerializer = new DataContractSerializer(typeof(Customer));
        static readonly DataContractSerializer listOfCustomersSerializer = new DataContractSerializer(typeof(List<Customer>));

        static void Main(string[] args)
        {
            Uri baseAddress = new Uri("http://localhost:33693/Customers");
            Console.WriteLine("Service is hosted at: " + baseAddress.AbsoluteUri);
            Console.WriteLine("Service help page is at: " + baseAddress.AbsoluteUri + "/help");
            Console.WriteLine("");

            Console.WriteLine("Adding some customers with POST:");
            Customer alice = new Customer("Alice", "123 Pike Place", null);
            Uri aliceLocation = PostCustomer(baseAddress, alice);

            Customer bob = new Customer("Bob", "2323 Lake Shore Drive", null);
            Uri bobLocation = PostCustomer(baseAddress, bob);

            Console.WriteLine("");

            Console.WriteLine("Using PUT to update a customer");
            alice.Name = "Charlie";
            alice.Uri = aliceLocation;
            PutCustomer(aliceLocation, alice);

            Console.WriteLine("");
            Console.WriteLine("Using GET to retrieve the list of customers");
            List<Customer> customers = GetCustomers(baseAddress);
            foreach (Customer c in customers)
            {
                Console.WriteLine(c.ToString());
            }

            Console.WriteLine("");
            Console.WriteLine("Using DELETE to delete a customer");
            DeleteCustomer(bobLocation);

            Console.WriteLine("");
            Console.WriteLine("Getting the list of customers once more. Since the cached response is returned the deleted customer still appears.");
            customers = GetCustomers(baseAddress);
            foreach (Customer c in customers)
            {
                Console.WriteLine(c.ToString());
            }

            Console.WriteLine("");

            Console.WriteLine("Sleeping for 1 minute so that the server cache is invalidated.");
            Thread.Sleep(TimeSpan.FromMinutes(1));
            Console.WriteLine("Getting the list of customers once more. Now the deleted customer does not appear.");
            customers = GetCustomers(baseAddress);
            foreach (Customer c in customers)
            {
                Console.WriteLine(c.ToString());
            }

            Console.WriteLine("Press any key to terminate");
            Console.ReadLine();
        }

        static Uri PostCustomer(Uri uri, Customer customer)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "POST";
            request.ContentType = "application/xml";
            using (Stream requestStream = request.GetRequestStream())
            {
                customerSerializer.WriteObject(requestStream, customer);
            }
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                Customer createdItem = (Customer) customerSerializer.ReadObject(responseStream);
                Console.WriteLine(createdItem.ToString());
            }
            response.Close();
            return new Uri(response.Headers[HttpResponseHeader.Location]);
        }

        static void PutCustomer(Uri uri, Customer customer)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "PUT";
            request.ContentType = "application/xml";
            using (Stream requestStream = request.GetRequestStream())
            {
                customerSerializer.WriteObject(requestStream, customer);
            }
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (Stream responseStream = response.GetResponseStream())
            {
                Customer updatedItem = (Customer)customerSerializer.ReadObject(responseStream);
                Console.WriteLine(updatedItem.ToString());
            }
            response.Close();
        }

        static List<Customer> GetCustomers(Uri uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            List<Customer> customers;
            using (Stream responseStream = response.GetResponseStream())
            {
                customers = (List<Customer>)listOfCustomersSerializer.ReadObject(responseStream);
            }
            response.Close();
            return customers;
        }

        static void DeleteCustomer(Uri uri)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "DELETE";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            response.Close();
        }
    }
}
