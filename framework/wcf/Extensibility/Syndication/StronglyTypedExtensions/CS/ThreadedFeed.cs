//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved. 
//----------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;

namespace Microsoft.Samples.StronglyTypedExtensions
{
    public class ThreadedFeed : SyndicationFeed
    {
        public ThreadedFeed()
        {
        }

        public IEnumerable<ThreadedItem> ThreadedItems
        {
            get
            {
                return this.Items.Cast<ThreadedItem>();
            }
        }

        protected override SyndicationItem CreateItem()
        {
            return new ThreadedItem();
        }
    }
}
