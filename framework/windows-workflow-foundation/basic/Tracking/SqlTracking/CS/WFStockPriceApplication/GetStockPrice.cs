//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Activities;
using System.Activities.Tracking;
using System.Collections.Generic;

namespace Microsoft.Samples.SqlTracking.WFStockPriceApplication
{

    public class GetStockPrice : CodeActivity
    {
        static Dictionary<string, double> knownSymbols = new Dictionary<string, double>()
        {
            {"Contoso",20},
            {"Northwind",10}
        };

        [RequiredArgument]
        public InArgument<string> StockSymbol { get; set; }
        [RequiredArgument]
        public OutArgument<double> Value { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            DateTime startLookup = DateTime.Now;

            string symbol = StockSymbol.Get(context);
            double price = knownSymbols[symbol];
            Value.Set(context, price);

            DateTime endLookup = DateTime.Now;

            TimeSpan lookupTime = endLookup - startLookup;
            CustomTrackingRecord userRecord = new CustomTrackingRecord("QuoteLookupEvent");
            userRecord.Data.Add("LookupTime", lookupTime.TotalMilliseconds);
            userRecord.Data.Add("Units", "Milliseconds");
            context.Track(userRecord);
        }
    }
}
