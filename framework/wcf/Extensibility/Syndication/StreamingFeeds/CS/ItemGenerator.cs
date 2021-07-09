//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved. 
//----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Text;

namespace Microsoft.Samples.StreamingFeeds
{
    class ItemGenerator
    {
        ItemCounter counter;
        long itemsReturned = 0;
        int maxItemsRead;
        Random random = new Random();

        public ItemGenerator(ItemCounter counter, int maxItemsRead)
        {
            this.maxItemsRead = maxItemsRead;
            this.counter = counter;
        }

        public IEnumerable<SyndicationItem> GenerateItems()
        {
            while (counter.GetCount() < maxItemsRead)
            {
                itemsReturned++;
                yield return CreateNextItem();
            }

        }

        SyndicationItem CreateNextItem()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < random.Next(1000); ++i)
            {
                builder.Append('c');
            }
            return new SyndicationItem("item " + this.itemsReturned.ToString(), builder.ToString(), null);
        }
    }
}
