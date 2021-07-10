//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;

namespace Microsoft.Samples.Tools.ConfigurationCodeGenerator
{
    // A structure to hold the PropertyName and PropertyType for properties we generate
    // for the custom BindingElement
    struct PropertyNameType
    {
        internal string propertyName;
        internal Type propertyType;
    }

    class TypeNameConstants
    {
        internal static readonly string Type = "Type";
        internal static readonly string String = "System.String";
        internal static readonly string Binding = "Binding";
        internal static readonly string BindingsSection = "BindingsSection";
        internal static readonly string CultureInfo = "CultureInfo";
        internal static readonly string BindingElement = "BindingElement";
        internal static readonly string StandardBindingElement = "StandardBindingElement";
        internal static readonly string ConfigurationManager = "ConfigurationManager";
        internal static readonly string ConfigurationProperty = "ConfigurationProperty";
        internal static readonly string ConfigurationPropertyCollection = "ConfigurationPropertyCollection";
        internal static readonly string BindingElementExtensionSection = "BindingElementExtensionSection";
        internal static readonly string ServiceModelExtensionSection = "ServiceModelExtensionSection";
        internal static readonly string StdBindingElement = "StandardBinding" + Constants.ElementSuffix;
        internal static readonly string StdBindingCollectionElement = "StandardBinding" + Constants.CollectionElementSuffix;
    }

    class MethodNameConstants
    {
        internal static readonly string AddMethod = "Add";
        internal static readonly string FormatMethod = "Format";
        internal static readonly string GetTypeMethod = "GetType";
        internal static readonly string CopyFromMethod = "CopyFrom";
        internal static readonly string GetSectionMethod = "GetSection";
        internal static readonly string InitializeFromMethod = "InitializeFrom";
        internal static readonly string ApplyConfigurationMethod = "ApplyConfiguration";
        internal static readonly string OnApplyConfigurationMethod = "OnApplyConfiguration";
        internal static readonly string CreateBindingElementMethod = "CreateBindingElement";
    }

    class PropertyNameConstants
    {
        internal static readonly string BindingsProperty = "Bindings";
        internal static readonly string BindingNameProperty = "BindingName";
        internal static readonly string PropertiesProperty = "Properties";
        internal static readonly string CurrentCultureProperty = "CurrentCulture";
        internal static readonly string BindingElementTypeProperty = "BindingElementType";
        internal static readonly string AssemblyQualifiedNameProperty = "AssemblyQualifiedName";
        internal static readonly string DefaultValueProperty = "DefaultValue";
    }

    class Constants
    {
        internal static readonly string ConfigurationStrings = "ConfigurationStrings";
        internal static readonly string ConfigurationPropertyOptionsNone = "None";
        internal static readonly string Defaults = "Defaults";
        internal static readonly string DefaultPrefix = "Default";
        internal static readonly string ElementSuffix = "Element";
        internal static readonly string CollectionElementSuffix = "CollectionElement";
        internal static readonly string bindingElementParamName = "bindingElement";
        internal static readonly string bindingParamName = "binding";
    }
}
