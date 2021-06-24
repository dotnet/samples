//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------
using System;
using System.Activities.Hosting;
using System.Activities.Persistence;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Samples.ContosoHR;

namespace Microsoft.Samples.HiringService
{
    public class HiringRequestInfoPersistenceParticipant: PersistenceIOParticipant
    {
        public HiringRequestInfoPersistenceParticipant()
            : base(true, false)
        {
        }

        protected override IAsyncResult BeginOnSave(IDictionary<System.Xml.Linq.XName, object> readWriteValues, IDictionary<System.Xml.Linq.XName, object> writeOnlyValues, TimeSpan timeout, AsyncCallback callback, object state)
        {            
            HiringRequestInfo info = ExtractHiringRequestInfoFromValues(writeOnlyValues);
            if (info != null)
            {
                HiringRequestRepository.Save(info);
            }            
            // save the hiring request data here
            return base.BeginOnSave(readWriteValues, writeOnlyValues, timeout, callback, state);
        }

        // Extracts the hiringRequest variable from the list of values to be persisted.
        // If not found, returns null
        HiringRequestInfo ExtractHiringRequestInfoFromValues(IDictionary<System.Xml.Linq.XName, object> writeOnlyValues)
        {
            try
            {
                // get the reference to the variable with the hiring request information
                LocationInfo loc = writeOnlyValues.Values.Single(o => o as LocationInfo != null &&
                                                                     ((LocationInfo)o).Name.Equals("hiringRequestInfo")) as LocationInfo;

                // save the HiringRequest information (if it is in the collection of values)
                if (loc != null && loc.Value is HiringRequestInfo)
                {
                    return (HiringRequestInfo)loc.Value;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        protected override void Abort()
        {
            throw new NotImplementedException();
        }
    }
}
