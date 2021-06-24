//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.CodeDom;
using System.Reflection;

namespace Microsoft.Samples.Tools.ConfigurationCodeGenerator
{
    class CodeDomHelperObjects
    {
        // frequently used access modifiers for methods and properties generated
        internal static MemberAttributes Public = MemberAttributes.Public;
        internal static MemberAttributes PublicFinal = MemberAttributes.Public | MemberAttributes.Final;
        internal static MemberAttributes InternalConstants = MemberAttributes.FamilyAndAssembly | MemberAttributes.Const;
        internal static MemberAttributes PublicOverride = MemberAttributes.Override | MemberAttributes.Public;
        internal static MemberAttributes ProtectedOverride = MemberAttributes.Override | MemberAttributes.Family;

        // frequently used CodeExpressions
        internal static CodeThisReferenceExpression ThisRef = new CodeThisReferenceExpression();
        internal static CodeBaseReferenceExpression BaseRef = new CodeBaseReferenceExpression();
        internal static CodePrimitiveExpression NullRef = new CodePrimitiveExpression(null);
        internal static CodeArgumentReferenceExpression bindingArgRef = new CodeArgumentReferenceExpression(Constants.bindingParamName);
        internal static CodeVariableReferenceExpression bindingVarRef = new CodeVariableReferenceExpression(Constants.bindingParamName);
        internal static CodeArgumentReferenceExpression bindingElementArgRef = new CodeArgumentReferenceExpression(Constants.bindingElementParamName);
        internal static CodeVariableReferenceExpression bindingElementVarRef = new CodeVariableReferenceExpression(Constants.bindingElementParamName);
        internal static CodePropertyReferenceExpression cultureInfoCurrent = new CodePropertyReferenceExpression(
                                                                new CodeTypeReferenceExpression(TypeNameConstants.CultureInfo),
                                                                PropertyNameConstants.CurrentCultureProperty);

        // frequently used CodeTypeReferences
        internal static CodeTypeReference bindingTypeRef = new CodeTypeReference(TypeNameConstants.Binding);
        internal static CodeTypeReference bindingElementTypeRef = new CodeTypeReference(TypeNameConstants.BindingElement);
        internal static CodeTypeReference typeTypeRef = new CodeTypeReference(TypeNameConstants.Type);
        internal static CodeTypeReference stringTypeRef = new CodeTypeReference(TypeNameConstants.String);

        // frequently used namespace imports
        internal static CodeNamespaceImport systemNS = new CodeNamespaceImport("System");
        internal static CodeNamespaceImport systemConfigNS = new CodeNamespaceImport("System.Configuration");
        internal static CodeNamespaceImport systemSMNS = new CodeNamespaceImport("System.ServiceModel");
        internal static CodeNamespaceImport systemSMChannelsNS = new CodeNamespaceImport("System.ServiceModel.Channels");
        internal static CodeNamespaceImport systemSMConfigNS = new CodeNamespaceImport("System.ServiceModel.Configuration");
        internal static CodeNamespaceImport systemGlobNS = new CodeNamespaceImport("System.Globalization");
    }

    class CodeGenHelperMethods
    {
        static CodeMemberProperty CreateProperty(PropertyNameType nameType, string constantsClassName, string defaultValuesClassName)
        {
            CodeMemberProperty publicProp = new CodeMemberProperty();
            publicProp.Name = nameType.propertyName;
            publicProp.Attributes = CodeDomHelperObjects.PublicFinal;
            publicProp.HasGet = true;
            publicProp.HasSet = true;
            publicProp.Type = new CodeTypeReference(nameType.propertyType);

            CodeAttributeDeclarationCollection attributes = new CodeAttributeDeclarationCollection();
            CodeAttributeArgument arg1 = new CodeAttributeArgument(
                                            new CodeFieldReferenceExpression(
                                                new CodeTypeReferenceExpression(constantsClassName),
                                                nameType.propertyName));
            CodeAttributeArgument arg2 = new CodeAttributeArgument(
                                            PropertyNameConstants.DefaultValueProperty,
                                            new CodeFieldReferenceExpression(
                                                new CodeTypeReferenceExpression(defaultValuesClassName),
                                                Constants.DefaultPrefix + nameType.propertyName));
            attributes.Add(new CodeAttributeDeclaration(
                            new CodeTypeReference(TypeNameConstants.ConfigurationProperty),
                            arg1, arg2));
            publicProp.CustomAttributes = new CodeAttributeDeclarationCollection(attributes);
            string nameInConfig = constantsClassName + "." + nameType.propertyName;
            CodeArrayIndexerExpression baseIndexedProperty
                                        = new CodeArrayIndexerExpression(
                                            CodeDomHelperObjects.BaseRef,
                                            new CodeFieldReferenceExpression(
                                                new CodeTypeReferenceExpression(constantsClassName),
                                                nameType.propertyName));
            publicProp.GetStatements.Add(new CodeMethodReturnStatement(
                                                new CodeCastExpression(
                                                    nameType.propertyType,
                                                    baseIndexedProperty)));
            publicProp.SetStatements.Add(new CodeAssignStatement(
                                                baseIndexedProperty,
                                                new CodePropertySetValueReferenceExpression()));
            return publicProp;
        }

        internal static CodeTypeDeclaration EmitDefaultValuesClass(PropertyNameType[] propertiesGenerated, string defaultValuesClassName)
        {
            CodeTypeDeclaration defaultValuesClass = new CodeTypeDeclaration(defaultValuesClassName);
            defaultValuesClass.TypeAttributes = TypeAttributes.NotPublic;
            MemberAttributes fieldAttrs = CodeDomHelperObjects.InternalConstants;
            CodeComment comment = new CodeComment("Please initialize default values to these fields");
            defaultValuesClass.Comments.Add(new CodeCommentStatement(comment));
            for (int i = 0; i < propertiesGenerated.Length; i++)
            {
                CodeMemberField field = new CodeMemberField(
                                            new CodeTypeReference(propertiesGenerated[i].propertyType.Name),
                                            Constants.DefaultPrefix + propertiesGenerated[i].propertyName);
                field.Attributes = fieldAttrs;
                defaultValuesClass.Members.Add(field);
            }
            return defaultValuesClass;
        }

        internal static CodeTypeDeclaration EmitConstantStringsClass(PropertyNameType[] propertiesGenerated, string constantsClassName)
        {
            CodeTypeDeclaration constantsClass = new CodeTypeDeclaration(constantsClassName);
            constantsClass.TypeAttributes = TypeAttributes.NotPublic;
            MemberAttributes fieldAttrs = CodeDomHelperObjects.InternalConstants;
            for (int i = 0; i < propertiesGenerated.Length; i++)
            {
                CodeMemberField field = new CodeMemberField(
                                            CodeDomHelperObjects.stringTypeRef,
                                            propertiesGenerated[i].propertyName);
                field.Attributes = fieldAttrs;
                field.InitExpression = new CodePrimitiveExpression(
                                            Helpers.TurnFirstCharLower(propertiesGenerated[i].propertyName));
                constantsClass.Members.Add(field);
            }
            return constantsClass;
        }

        internal static void EmitReflectedProperties(ref CodeTypeDeclaration customConfigCodeClass, Type typeToReflect, out PropertyNameType[] propertiesGenerated, string constantsClassName, string defaultValuesClassName)
        {
            EmitReflectedProperties(ref customConfigCodeClass, typeToReflect,
                                    out propertiesGenerated, false, constantsClassName, defaultValuesClassName);
        }

        internal static void EmitReflectedProperties(ref CodeTypeDeclaration customConfigCodeClass, Type typeToReflect, out PropertyNameType[] propertiesGenerated, bool excludeBaseClassProperties, string constantsClassName, string defaultValuesClassName)
        {
            // by default, include base classes
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;
            // if asked to exclude, modify BindingFlags
            if (excludeBaseClassProperties)
            {
                bindingFlags = bindingFlags | BindingFlags.DeclaredOnly;
            }

            PropertyInfo[] allProperties = typeToReflect.GetProperties(bindingFlags);
            PropertyNameType[] allPropNames = new PropertyNameType[allProperties.Length];
            int index = 0;
            foreach (PropertyInfo propertyInfo in allProperties)
            {
                // Only expose properties in config that can be written
                if (propertyInfo.CanWrite)
                {
                    // Including types present in base classes as well
                    PropertyNameType nameType;
                    nameType.propertyName = propertyInfo.Name;
                    nameType.propertyType = propertyInfo.PropertyType;

                    customConfigCodeClass.Members.Add(CodeGenHelperMethods.CreateProperty(nameType, constantsClassName, defaultValuesClassName));
                    allPropNames.SetValue(nameType, index++);
                }
            }
            propertiesGenerated = new PropertyNameType[index];
            Array.Copy(allPropNames, propertiesGenerated, index);
        }

        internal static void EmitPropertiesProperty(ref CodeTypeDeclaration customConfigCodeClass, PropertyNameType[] propertiesGenerated, string constantsClassName, string defaultValuesClassName)
        {
            CodeTypeReference configPropCollectionType = new CodeTypeReference(
                                                TypeNameConstants.ConfigurationPropertyCollection);
            CodeMemberProperty propertiesProperty = new CodeMemberProperty();
            propertiesProperty.Name = PropertyNameConstants.PropertiesProperty;
            propertiesProperty.Attributes = CodeDomHelperObjects.ProtectedOverride;
            propertiesProperty.HasSet = false;
            propertiesProperty.HasGet = true;
            propertiesProperty.Type = configPropCollectionType;

            string varProperties = Helpers.TurnFirstCharLower(PropertyNameConstants.PropertiesProperty);
            CodeVariableDeclarationStatement cvds = new CodeVariableDeclarationStatement(
                                                        configPropCollectionType,
                                                        varProperties,
                                                        new CodePropertyReferenceExpression(
                                                            CodeDomHelperObjects.BaseRef,
                                                            PropertyNameConstants.PropertiesProperty));
            propertiesProperty.GetStatements.Add(cvds);

            CodeVariableReferenceExpression varPropsRef = new CodeVariableReferenceExpression(varProperties);

            foreach (PropertyNameType nameType in propertiesGenerated)
            {
                CodeObjectCreateExpression paramToAddMethod = new CodeObjectCreateExpression(
                                                                TypeNameConstants.ConfigurationProperty,
                                                                new CodeFieldReferenceExpression(
                                                                    new CodeTypeReferenceExpression(constantsClassName),
                                                                    nameType.propertyName),
                                                                new CodeTypeOfExpression(
                                                                    new CodeTypeReference(nameType.propertyType)),
                                                                new CodeFieldReferenceExpression(
                                                                    new CodeTypeReferenceExpression(defaultValuesClassName),
                                                                    Constants.DefaultPrefix + nameType.propertyName));
                CodeMethodInvokeExpression cmie = new CodeMethodInvokeExpression(
                                                        varPropsRef,
                                                        MethodNameConstants.AddMethod,
                                                        paramToAddMethod);
                propertiesProperty.GetStatements.Add(cmie);
            }

            propertiesProperty.GetStatements.Add(new CodeMethodReturnStatement(varPropsRef));
            customConfigCodeClass.Members.Add(propertiesProperty);
        }
    }
}
