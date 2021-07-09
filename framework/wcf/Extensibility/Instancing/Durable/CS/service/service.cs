
//  Copyright (c) Microsoft Corporation.  All Rights Reserved.

using System;
using System.ServiceModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.ServiceModel.Samples
{
    // Define a service contract.
    [ServiceContract(Namespace="http://Microsoft.ServiceModel.Samples", SessionMode=SessionMode.Required)]
    public interface IShoppingCart
    {
        [OperationContract]
        int AddItem(string item);

        [OperationContract]
        int Clear();

        [OperationContract]
        int RemoveItem(string item);

        [OperationContract]
        int ItemCount();

        [OperationContract]
        Collection<string> GetCart();
    }

    // Shopping cart implementation.
    [DurableInstanceContext]
    [ServiceBehavior(InstanceContextMode=InstanceContextMode.PerSession)]
    public class ShoppingCart : IShoppingCart
    {
        Collection<string> items;

        public Collection<string> Items
        {
            get
            {
                if (items == null)
                    items = new Collection<string>();
                return items;
            }
        }

        [SaveState]
        public int AddItem(string item)
        {
            Items.Add(item);
            return ItemCount();
        }

        [SaveState]
        public int Clear()
        {
            Items.Clear();
            return 0;
        }

        [SaveState]
        public int RemoveItem(string item)
        {
            Items.Remove(item);
            return ItemCount();
        }

        public int ItemCount()
        {
            return Items.Count;
        }

        public Collection<string> GetCart()
        {
            return Items;
        }

        // Host the service within this EXE console application.
        public static void Main()
        {
            // Create a ServiceHost for the CalculatorService type.
            using (ServiceHost serviceHost = new ServiceHost(typeof(ShoppingCart)))
            {
                // Open the ServiceHost to create listeners and start listening for messages.
                serviceHost.Open();

                // The service can now be accessed.
                Console.WriteLine("The service is ready.");
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();

            }
        }
    }
}

