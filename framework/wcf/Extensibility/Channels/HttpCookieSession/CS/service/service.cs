//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.Collections.Generic;
using System.ServiceModel;

namespace Microsoft.Samples.HttpCookieSession
{
    // ShoppingCart contract.
    [ServiceContract(Namespace="http://Microsoft.Samples.HttpCookieSession", SessionMode=SessionMode.Required)]
    public interface IShoppingCart
    {
        [OperationContract]
        int AddItem(int itemId, int quantity);
        [OperationContract]
        int Clear();
        [OperationContract]
        int RemoveItem(int itemId, int quantity);
        [OperationContract]
        int ItemCount();
        [OperationContract]
        Dictionary<int, int> GetItems();
    }

    // Service implementation of the IShoppingCart contract.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class ShoppingCartService : IShoppingCart
    {
        Dictionary<int, int> shoppingCart = new Dictionary<int, int>();

        public ShoppingCartService()
        {
        }

        Dictionary<int, int> ShoppingCart
        {
            get
            {
                return shoppingCart;
            }
        }

        public int AddItem(int itemId, int quantity)
        {
            if (ShoppingCart.ContainsKey(itemId))
            {
                ShoppingCart[itemId] += quantity;
            }
            else
            {
                ShoppingCart.Add(itemId, quantity);
            }
            return ItemCount();
        }

        public int Clear()
        {
            ShoppingCart.Clear();
            return 0;
        }

        public int RemoveItem(int itemId, int quantity)
        {
            if (ShoppingCart.ContainsKey(itemId))
            {
                if (ShoppingCart[itemId] > quantity)
                    ShoppingCart[itemId] -= quantity;
                else
                    ShoppingCart.Remove(itemId);

            }
            return ItemCount();
        }

        public int ItemCount()
        {
            int count = 0;
            if (ShoppingCart.Count > 0)
            {
                foreach (KeyValuePair<int, int> item in ShoppingCart)
                {
                    count += item.Value;
                }
            }
            return count;
        }

        public Dictionary<int, int> GetItems()
        {
            return ShoppingCart;
        }
    }
}
