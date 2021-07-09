//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Microsoft.Samples.ReliableSecureProfile
{
    public sealed class MakeConnectionDispatcherManager
    {
        Dictionary<Uri, MakeConnectionDispatcher> dispatchers;

        public MakeConnectionDispatcherManager()
        {
            this.dispatchers = new Dictionary<Uri, MakeConnectionDispatcher>();
        }

        public MakeConnectionDispatcher Get(Uri uri)
        {
            MakeConnectionDispatcher dispatcher = null;

            lock (dispatchers)
            {
                if (!dispatchers.TryGetValue(uri, out dispatcher))
                {
                    dispatcher = new MakeConnectionDispatcher(uri, dispatchers);
                    dispatchers.Add(uri, dispatcher);
                    dispatcher.ReferencesReleased += new EventHandler(OnReferencesReleased);
                }

                dispatcher.AddRef();
            }

            return dispatcher;
        }

        public void Shutdown()
        {
            lock (dispatchers)
            {
                MakeConnectionDispatcher[] arrayOfDispatchers = new MakeConnectionDispatcher[dispatchers.Count];

                dispatchers.Values.CopyTo(arrayOfDispatchers, 0);
                for (int i = 0; i < arrayOfDispatchers.Length; i++)
                {
                    arrayOfDispatchers[i].Shutdown();
                    dispatchers.Remove(arrayOfDispatchers[i].Uri);
                }
            }
        }

        // this is invoked under a lock
        void OnReferencesReleased(object sender, EventArgs e)
        {
            Uri key = ((MakeConnectionDispatcher)sender).Uri;
            if (dispatchers.ContainsKey(key))
            {
                dispatchers.Remove(key);
            }
        }
    }
}
