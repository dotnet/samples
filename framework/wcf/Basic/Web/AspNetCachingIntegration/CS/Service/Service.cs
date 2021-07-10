//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace Microsoft.Samples.AspNetCachingIntegration
{
    [ServiceContract, ServiceBehavior(InstanceContextMode = InstanceContextMode.Single), AspNetCompatibilityRequirements(RequirementsMode=AspNetCompatibilityRequirementsMode.Allowed)]
    public class Service
    {
        int counter = 0;
        Hashtable customers = new Hashtable();
        object writeLock = new Object();

        [WebInvoke(Method = "POST", UriTemplate = ""), Description("Adds a customer to customers collection. The response Location header contains a URL to the added item.")]
        public Customer AddCustomer(Customer customer)
        {
            lock (writeLock)
            {
                counter++;
                UriTemplate itemTemplate = WebOperationContext.Current.GetUriTemplate("GetCustomer");
                customer.Uri = itemTemplate.BindByPosition(WebOperationContext.Current.IncomingRequest.UriTemplateMatch.BaseUri, counter.ToString());
                customers[counter.ToString()] = customer;
                WebOperationContext.Current.OutgoingResponse.SetStatusAsCreated(customer.Uri);
            }

            return customer;
        }

        [WebInvoke(Method = "DELETE", UriTemplate = "{id}"), Description("Deletes the specified customer from customers collection. Returns NotFound if there is no such customer.")]
        public void DeleteCustomer(string id)
        {
            if (!customers.ContainsKey(id))
            {
                throw new WebFaultException(HttpStatusCode.NotFound);
            }
            else
            {
                lock (writeLock)
                {
                    customers.Remove(id);
                }
            }
        }

        [WebGet(UriTemplate = "{id}"), Description("Returns the specified customer from customers collection. The response is cached for 1 minute. Returns NotFound if there is no such customer.")]
        [AspNetCacheProfile("CacheFor60Seconds")]
        public Customer GetCustomer(string id)
        {
            Customer c = this.customers[id] as Customer;

            if (c == null)
            {
                throw new WebFaultException(HttpStatusCode.NotFound);
            }

            return c;
        }


        [WebGet(UriTemplate = ""), Description("Returns all the customers in the customers collection. The response is cached for 1 minute.")]
        [AspNetCacheProfile("CacheFor60Seconds")]
        public List<Customer> GetCustomers()
        {
            List<Customer> list = new List<Customer>();

            lock (writeLock)
            {
                foreach (Customer c in this.customers.Values)
                {
                    list.Add(c);
                }
            }

            return list;
        }

        [WebInvoke(Method = "PUT", UriTemplate = "{id}"), Description("Updates the specified customer. Returns NotFound if there is no such customer.")]
        public Customer UpdateCustomer(string id, Customer newCustomer)
        {
            if (!customers.ContainsKey(id))
            {
                throw new WebFaultException(HttpStatusCode.NotFound);
            }

            lock (writeLock)
            {
                customers[id] = newCustomer;
            }

            return newCustomer;
        }

    }
}
