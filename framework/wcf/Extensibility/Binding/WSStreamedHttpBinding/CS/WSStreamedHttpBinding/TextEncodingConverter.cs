//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Text;
using System.Globalization;

namespace Microsoft.Samples.WSStreamedHttpBinding
{
    class TextEncodingConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (typeof(string) == sourceType)
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (typeof(InstanceDescriptor) == destinationType)
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                if (null == value)
                {
                    throw new ArgumentNullException("value");
                }
                string encoding = (string)value;
                Encoding retval = Encoding.GetEncoding(encoding);
                if (retval == null)
                {
                    throw new ArgumentException();
                }
                return retval;
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (typeof(string) == destinationType && value is Encoding)
            {
                if (null == value)
                {
                    throw new ArgumentNullException("value");
                }
                Encoding encoding = (Encoding)value;
                return encoding.HeaderName;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
