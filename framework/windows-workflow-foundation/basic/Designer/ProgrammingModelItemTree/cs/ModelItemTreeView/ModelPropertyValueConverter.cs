//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------


using System;
using System.Activities;
using System.Activities.Presentation.Model;
using System.Collections.Generic;
using System.Windows.Data;

namespace Microsoft.Samples.Activities.Designer.ProgrammingModelItemTree
{
    public class ModelPropertyValueConverter :IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            
            ModelItem mi = value as ModelItem;

            if (null != mi)
            {
                // in order to generate a child node in the treeview
                // we need to return a collection, so fix up a few types of interest
                if (mi.ItemType.Equals(typeof(string)))
                    return new List<object> { mi.GetCurrentValue() };
                if (mi.ItemType.IsSubclassOf(typeof(Activity)) || mi.ItemType.IsSubclassOf(typeof(ActivityDelegate)))
                    return new List<object> { mi };
                return mi;
            }
            else
            {
                return value;
            }
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
