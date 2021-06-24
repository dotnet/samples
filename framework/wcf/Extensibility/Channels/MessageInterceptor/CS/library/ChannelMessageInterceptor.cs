//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Microsoft.Samples.MessageInterceptor
{
    public abstract class ChannelMessageInterceptor
    {
        public virtual void OnSend(ref Message message){}
        public virtual void OnReceive(ref Message message){}

        public virtual void OnExportPolicy(MetadataExporter exporter, PolicyConversionContext context){}

        public virtual void OnImportPolicy(MetadataImporter importer, PolicyConversionContext context){}

        public abstract ChannelMessageInterceptor Clone();
    }
}
