//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Activities;
using System.Activities.Hosting;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xaml;

namespace Microsoft.Samples.WPFWFIntegration
{

    public static class SymbolResolverExtensions
    {
        public static SymbolResolver EnvironmentFromNameTable(IXamlNameResolver table, SymbolResolver parent)
        {
            SymbolResolver environment = parent ?? new SymbolResolver();

            if (table == null) 
            { 
                return environment; 
            }

            foreach (KeyValuePair<string, object> pair in table.GetAllNamesAndValuesInScope())
            {
                environment.Add(pair);
            }

            return environment;
        }

        public static SymbolResolver EnvironmentFromObject(object dataObject, SymbolResolver parent)
        {

            // If we have no data context, return an empty environment.
            //
            if (dataObject == null) 
            { 
                return parent ?? new SymbolResolver(); 
            }

            // Maybe it is already an environment.
            //
            SymbolResolver environment = dataObject as SymbolResolver;
            if (environment != null) 
            {
                return environment; 
            }

            // Otherwise populate the new environment based on the data context.
            //
            environment = parent ?? new SymbolResolver();
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(dataObject))
            {
                if (property.IsReadOnly)
                {
                    environment.Add(property.Name, property.GetValue(dataObject));
                }
                else
                {
                    environment.Add(property.Name, new PropertyDescriptorLocationReference { Component = dataObject, Property = property });  
                }
            }

            return environment;
        }

        class PropertyDescriptorLocationReference : Location, INotifyPropertyChanged
        {
            object component;
            PropertyDescriptor property;

            public object Component
            {
                get { return this.component; }
                set
                {
                    Unsubscribe();
                    this.component = value;
                    Subscribe();
                }
            }

            public PropertyDescriptor Property
            {
                get { return this.property; }
                set { this.property = value; }
            }

            public override Type LocationType
            {
                get { return this.property.PropertyType; }
            }

            protected override object ValueCore
            {
                get
                {
                    return this.property.GetValue(this.component);
                }
                set
                {
                    this.property.SetValue(this.component, value);
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            void ComponentPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == this.property.Name)
                {
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Value"));
                    }
                }
            }

            void Subscribe()
            {
                INotifyPropertyChanged inpc = this.component as INotifyPropertyChanged;
                if (inpc != null)
                {
                    inpc.PropertyChanged += ComponentPropertyChanged;
                }
            }

            void Unsubscribe()
            {
                INotifyPropertyChanged inpc = this.component as INotifyPropertyChanged;
                if (inpc != null)
                {
                    inpc.PropertyChanged -= ComponentPropertyChanged;
                }
            }
        }
    }
}
