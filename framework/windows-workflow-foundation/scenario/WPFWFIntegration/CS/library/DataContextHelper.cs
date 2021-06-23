//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Samples.WPFWFIntegration
{

    public static class DataContextHelper
    {
        public static bool HasDataContext(object targetObject)
        {
            FrameworkElement frameworkElement = targetObject as FrameworkElement;
            if (frameworkElement != null) 
            { 
                return true; 
            }

            FrameworkContentElement frameworkContentElement = targetObject as FrameworkContentElement;
            if (frameworkContentElement != null)
            { 
                return true; 
            }

            return false;
        }

        public static object GetDataContext(object targetObject)
        {
            FrameworkElement frameworkElement = targetObject as FrameworkElement;
            if (frameworkElement != null) 
            { 
                return frameworkElement.DataContext; 
            }

            FrameworkContentElement frameworkContentElement = targetObject as FrameworkContentElement;
            if (frameworkContentElement != null) 
            { 
                return frameworkContentElement.DataContext; 
            }

            throw new System.ArgumentException("TargetObject must have a DataContext. Try calling TryGetDataContext() instead.");
        }

        public static bool TryGetDataContext(object targetObject, out object dataContext)
        {
            dataContext = null;

            FrameworkElement frameworkElement = targetObject as FrameworkElement;
            if (frameworkElement != null) 
            { 
                dataContext = frameworkElement.DataContext; return true; 
            }

            FrameworkContentElement frameworkContentElement = targetObject as FrameworkContentElement;
            if (frameworkContentElement != null)
            { 
                dataContext = frameworkContentElement.DataContext; return true; 
            }

            return false;
        }

        public static Func<object> GetDataContextProvider(object targetObject)
        {
            Func<object> dataContextProvider = null;

            if (targetObject != null && HasDataContext(targetObject))
            {
                dataContextProvider = () =>
                    {
                        object dataContext;
                        bool sanityCheck = TryGetDataContext(targetObject, out dataContext);
                        Debug.Assert(sanityCheck);

                        return dataContext;
                    };
            }


            if (dataContextProvider == null)
            {
                dataContextProvider = () => null;
            }

            return dataContextProvider;
        }

        internal static object GetCurrentItem(object dataObject)
        {
            IListSource source = dataObject as IListSource;
            if (source != null)
            {
                dataObject = source.GetList();
            }

            if ((dataObject is IEnumerable) && !(dataObject is string))
            {
                return CollectionViewSource.GetDefaultView(dataObject).CurrentItem;
            }
            else
            {
                return dataObject;
            }
        }
    }
}
