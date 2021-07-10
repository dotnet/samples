//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;

namespace Microsoft.Samples.FormPost
{
    class Program
    {
        static readonly DataContractSerializer customerSerializer = new DataContractSerializer(typeof(Customer));
        static readonly DataContractSerializer listOfCustomersSerializer = new DataContractSerializer(typeof(List<Customer>));

        static void Main(string[] args)
        {
            using (WebServiceHost host = new WebServiceHost(typeof(Service), new Uri("http://localhost:8000/Customers")))
            {
                // Add and configure the service endpoint
                ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(Service), new WebHttpBinding(), "");
                HtmlFormProcessingBehavior formProcessingBehavior = new HtmlFormProcessingBehavior();
                formProcessingBehavior.HelpEnabled = true;
                endpoint.Behaviors.Add(formProcessingBehavior);

                host.Open();

                Uri baseAddress = new Uri("http://localhost:8000/Customers");
                Console.WriteLine("Service is hosted at: " + baseAddress.AbsoluteUri);
                Console.WriteLine("Service help page is at: " + baseAddress.AbsoluteUri + "/help");
                Console.WriteLine("");

                Console.WriteLine("Adding some customers with POST:");
                Customer alice = new Customer("Alice", "123 Pike Place", null);
                Uri aliceLocation = PostCustomer(baseAddress, alice);

                Customer bob = new Customer("Bob", "2323 Lake Shore Drive", null);
                Uri bobLocation = PostCustomer(baseAddress, bob);

                Console.WriteLine("");

                Console.WriteLine("Browse to http://localhost:8000/Customers/Form to add additional customers using an HTML form.");
                Console.WriteLine("");
                Console.WriteLine("Press any key to continue");
                Console.ReadLine();

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
                Console.WriteLine("Final list of customers: ");
                customers = GetCustomers(baseAddress);
                foreach (Customer c in customers)
                {
                    Console.WriteLine(c.ToString());
                }

                Console.WriteLine("");

                Console.WriteLine("Press any key to terminate");
                Console.ReadLine();
            }
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
