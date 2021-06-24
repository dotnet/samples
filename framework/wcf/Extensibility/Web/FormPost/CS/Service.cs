//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;

namespace Microsoft.Samples.FormPost
{
    [ServiceContract, ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Service
    {
        int counter = 0;
        Hashtable customers = new Hashtable();
        object writeLock = new Object();

        [WebGet(UriTemplate = "Form"), Description("Returns an HTML form for adding a customer to customers collection.")]
        public Message GetCustomerForm()
        {
            CustomerForm form = new CustomerForm();

            form.PostUri = WebOperationContext.Current.GetUriTemplate("AddCustomer").BindByPosition(
                WebOperationContext.Current.IncomingRequest.UriTemplateMatch.BaseUri).ToString();

            return WebOperationContext.Current.CreateTextResponse(form.TransformText(), "text/html");
        }

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

        [WebGet(UriTemplate = "{id}"), Description("Returns the specified customer from customers collection. Returns NotFound if there is no such customer.")]
        public Customer GetCustomer(string id)
        {
            Customer c = this.customers[id] as Customer;

            if (c == null)
            {
                throw new WebFaultException(HttpStatusCode.NotFound);
            }

            return c;
        }


        [WebGet(UriTemplate = ""), Description("Returns all the customers in the customers collection.")]
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
