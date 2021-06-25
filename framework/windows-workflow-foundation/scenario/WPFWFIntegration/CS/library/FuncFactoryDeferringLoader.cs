//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Reflection;
using System.Xaml;

namespace Microsoft.Samples.WPFWFIntegration
{

    class FuncFactoryDeferringLoader : XamlDeferringLoader
    {
        public override object Load(XamlReader xamlReader, IServiceProvider serviceProvider)
        {
            IXamlObjectWriterFactory settings = serviceProvider.GetService(typeof(IXamlObjectWriterFactory)) as IXamlObjectWriterFactory;

            System.Windows.Markup.IProvideValueTarget provideValueService = serviceProvider.GetService(typeof(System.Windows.Markup.IProvideValueTarget)) as System.Windows.Markup.IProvideValueTarget;

            Type propertyType = null;
                        
            // IProvideValueTarget.TargetProperty can return DP, Attached Property or MemberInfo for clr property
            // In this case it should always be a regular clr property here.
            PropertyInfo propertyInfo = provideValueService.TargetProperty as PropertyInfo;

            if (propertyInfo != null)
            {
                propertyType = propertyInfo.PropertyType;
            }
            else
            {
                return null;
            }

            object instance = Activator.CreateInstance(
                typeof(FuncFactory<>).MakeGenericType(propertyType.GetGenericArguments()),
                settings,
                xamlReader);

            return Delegate.CreateDelegate(propertyType, instance, instance.GetType().GetMethod("Evaluate"));

        }

        public override XamlReader Save(object value, IServiceProvider serviceProvider)
        {
            return ((FuncFactory)((Delegate)value).Target).Nodes.GetReader();
        }
    }

    abstract class FuncFactory
    {
        internal XamlNodeList Nodes { get; set; }
    }

    class FuncFactory<TReturn> : FuncFactory
    {
        IXamlObjectWriterFactory context;

        public FuncFactory(IXamlObjectWriterFactory context, XamlReader reader)
        {
            this.context = context;
            this.Nodes = new XamlNodeList(reader.SchemaContext);
            XamlServices.Transform(reader, this.Nodes.Writer);
        }

        public TReturn Evaluate()
        {
            XamlObjectWriter objectWriter = this.context.GetXamlObjectWriter(null);
            XamlServices.Transform(Nodes.GetReader(), objectWriter);
            return (TReturn)objectWriter.Result;
        }
    }
}
