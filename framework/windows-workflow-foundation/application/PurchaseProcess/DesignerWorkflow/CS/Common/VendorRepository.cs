//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System.Collections.Generic;

namespace Microsoft.Samples.WF.PurchaseProcess
{

    /// <summary>
    /// Repository of Vendor instances
    /// </summary>
    public static class VendorRepository
    {
        static IDictionary<int, Vendor> allVendors = new Dictionary<int, Vendor>
            {                
                { 1, new Vendor(1, "Vendor 1", 95) },
                { 2, new Vendor(2, "Vendor 2", 60) },
                { 3, new Vendor(3, "Vendor 3", 90) },
                { 4, new Vendor(4, "Vendor 4", 92) },
                { 5, new Vendor(5, "Vendor 5", 86) },
                { 6, new Vendor(6, "Vendor 6", 57) },
                { 7, new Vendor(7, "Vendor 7", 77) },
                { 8, new Vendor(8, "Vendor 8", 83) }
            };

        public static ICollection<Vendor> RetrieveAll()
        {
            return allVendors.Values;
        }

        public static Vendor Retrieve(int id)
        {
            if (allVendors.ContainsKey(id))
            {
                return allVendors[id];
            }
            else
            {
                return null;
            }
        }
    }
}
