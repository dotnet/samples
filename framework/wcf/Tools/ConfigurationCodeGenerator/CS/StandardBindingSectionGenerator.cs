//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Reflection;

namespace Microsoft.Samples.Tools.ConfigurationCodeGenerator
{
    // This will generate the config section for the StandardBinding
    class StandardBindingSectionGenerator
    {
        CodeCompileUnit compileUnit;
        Type standardBindingType;
        Assembly userAssembly;
        CodeDomProvider provider;

        // The name of the classes we will generate
        string generatedElementClassName;
        string generatedCollectionElementClassName;

        // The namespace of the generated class
        string namespaceOfGenClass;

        // class we'll generate to hold some constants
        string constantsClassName;

        // A class with default values of properties that we will generate
        string defaultValuesClassName;
        
        // Name and Type of all public properties we'll generate for the custom BindingElement
        PropertyNameType[] propertiesGenerated;

        CodeTypeReference customSBTypeRef;
        CodeTypeOfExpression customSBTypeOfRef;
        CodeParameterDeclarationExpression bindingMethodParamRef;

        internal StandardBindingSectionGenerator(Type standardBindingType, Assembly userAssembly, CodeDomProvider provider)
        {
            this.standardBindingType = standardBindingType;
            this.userAssembly = userAssembly;
            this.provider = provider;

            this.generatedElementClassName = standardBindingType.Name + Constants.ElementSuffix;
            this.constantsClassName = standardBindingType.Name.Substring(0, standardBindingType.Name.IndexOf(TypeNameConstants.Binding)) + Constants.ConfigurationStrings;
            this.defaultValuesClassName = standardBindingType.Name.Substring(0, standardBindingType.Name.IndexOf(TypeNameConstants.Binding)) + Constants.Defaults;
            this.generatedCollectionElementClassName = standardBindingType.Name + Constants.CollectionElementSuffix;

            this.customSBTypeRef = new CodeTypeReference(standardBindingType.Name);
            this.customSBTypeOfRef = new CodeTypeOfExpression(customSBTypeRef);
            this.bindingMethodParamRef = new CodeParameterDeclarationExpression(
                                            CodeDomHelperObjects.bindingTypeRef, 
                                            Constants.bindingParamName);
        }

        CodeNamespace EmitNamespaceAndUsings()
        {
            CodeNamespace nameSpace = new CodeNamespace(standardBindingType.Namespace);
            namespaceOfGenClass = nameSpace.Name;

            nameSpace.Imports.Add(CodeDomHelperObjects.systemNS);
            nameSpace.Imports.Add(CodeDomHelperObjects.systemConfigNS);
            nameSpace.Imports.Add(CodeDomHelperObjects.systemSMNS);
            nameSpace.Imports.Add(CodeDomHelperObjects.systemSMChannelsNS);
            nameSpace.Imports.Add(CodeDomHelperObjects.systemSMConfigNS);
            nameSpace.Imports.Add(CodeDomHelperObjects.systemGlobNS);

            return nameSpace;
        }

        CodeTypeDeclaration EmitClass()
        {
            CodeTypeDeclaration customSBConfigSectionClass = new CodeTypeDeclaration(generatedElementClassName);
            CodeTypeReference baseClass = new CodeTypeReference(TypeNameConstants.StdBindingElement);
            customSBConfigSectionClass.BaseTypes.Add(baseClass);
            return customSBConfigSectionClass;
        }

        CodeTypeDeclaration EmitCollectionElementClass()
        {
            CodeTypeDeclaration customSBCollectionElementClass = new CodeTypeDeclaration(generatedCollectionElementClassName);
            CodeTypeReference baseClass = new CodeTypeReference(
                                                TypeNameConstants.StdBindingCollectionElement,
                                                customSBTypeRef,
                                                new CodeTypeReference(generatedElementClassName));
            customSBCollectionElementClass.BaseTypes.Add(baseClass);
            return customSBCollectionElementClass;
        }

        void EmitConstructors(ref CodeTypeDeclaration customSBConfigSectionClass)
        {
            CodeConstructor stringCons = new CodeConstructor();
            stringCons.Attributes = CodeDomHelperObjects.Public;
            string paramConfigName = "configurationName";
            CodeVariableReferenceExpression varConfigNameRef = new CodeVariableReferenceExpression(paramConfigName);
            stringCons.Parameters.Add(new CodeParameterDeclarationExpression(
                    new CodeTypeReference(TypeNameConstants.String), paramConfigName));
            stringCons.BaseConstructorArgs.Add(varConfigNameRef);
            customSBConfigSectionClass.Members.Add(stringCons);

            CodeConstructor noArgsCons = new CodeConstructor();
            noArgsCons.Attributes = CodeDomHelperObjects.Public;
            noArgsCons.ChainedConstructorArgs.Add(CodeDomHelperObjects.NullRef);
            customSBConfigSectionClass.Members.Add(noArgsCons);
        }

        void EmitBindingElementTypeProperty(ref CodeTypeDeclaration customSBConfigSectionClass)
        {
            CodeMemberProperty bindingElementTypeProperty = new CodeMemberProperty();
            bindingElementTypeProperty.Name = PropertyNameConstants.BindingElementTypeProperty;
            bindingElementTypeProperty.Attributes = CodeDomHelperObjects.ProtectedOverride;
            bindingElementTypeProperty.HasSet = false;
            bindingElementTypeProperty.HasGet = true;
            bindingElementTypeProperty.Type = CodeDomHelperObjects.typeTypeRef;
            bindingElementTypeProperty.GetStatements.Add(new CodeMethodReturnStatement(customSBTypeOfRef));
            customSBConfigSectionClass.Members.Add(bindingElementTypeProperty);
        }

        void EmitInitializeFromMethod(ref CodeTypeDeclaration customSBConfigSectionClass)
        {
            CodeMemberMethod initFromMethod = new CodeMemberMethod();
            initFromMethod.Name = MethodNameConstants.InitializeFromMethod;
            initFromMethod.Parameters.Add(bindingMethodParamRef);
            initFromMethod.Attributes = CodeDomHelperObjects.ProtectedOverride;
            CodeMethodInvokeExpression invokeBase =
                new CodeMethodInvokeExpression(CodeDomHelperObjects.BaseRef,
                    MethodNameConstants.InitializeFromMethod, CodeDomHelperObjects.bindingArgRef);
            initFromMethod.Statements.Add(invokeBase);
            string varInstanceName = Helpers.TurnFirstCharLower(standardBindingType.Name);
            CodeVariableDeclarationStatement cvds = new CodeVariableDeclarationStatement(
                customSBTypeRef, varInstanceName,
                new CodeCastExpression(standardBindingType.Name, CodeDomHelperObjects.bindingVarRef));
            initFromMethod.Statements.Add(cvds);

            foreach (PropertyNameType nameType in propertiesGenerated)
            {
                CodeAssignStatement assignment = new CodeAssignStatement(
                                                    new CodeFieldReferenceExpression(
                                                        CodeDomHelperObjects.ThisRef,
                                                        nameType.propertyName),
                                                    new CodeFieldReferenceExpression(
                                                        new CodeVariableReferenceExpression(varInstanceName),
                                                        nameType.propertyName)
                                                    );
                initFromMethod.Statements.Add(assignment);
            }
            customSBConfigSectionClass.Members.Add(initFromMethod);
        }

        void EmitOnApplyConfigurationMethod(ref CodeTypeDeclaration customSBConfigSectionClass)
        {
            CodeMemberMethod onApplyCfgMethod = new CodeMemberMethod();
            onApplyCfgMethod.Name = MethodNameConstants.OnApplyConfigurationMethod; ;
            onApplyCfgMethod.Parameters.Add(bindingMethodParamRef);
            onApplyCfgMethod.Attributes = CodeDomHelperObjects.ProtectedOverride;
            CodeBinaryOperatorExpression cboe = new CodeBinaryOperatorExpression(
                                                    CodeDomHelperObjects.bindingVarRef,
                                                    CodeBinaryOperatorType.IdentityEquality,
                                                    CodeDomHelperObjects.NullRef);
            CodeThrowExceptionStatement ctes = new CodeThrowExceptionStatement(
                                                    new CodeObjectCreateExpression(
                                                        new CodeTypeReference(typeof(System.ArgumentNullException)),
                                                        new CodePrimitiveExpression(Constants.bindingParamName)));
            CodeConditionStatement ccs = new CodeConditionStatement(cboe, ctes);
            onApplyCfgMethod.Statements.Add(ccs);
            CodeMethodInvokeExpression cmie = new CodeMethodInvokeExpression(
                                                CodeDomHelperObjects.bindingVarRef, 
                                                MethodNameConstants.GetTypeMethod);
            cboe = new CodeBinaryOperatorExpression(
                        cmie,
                        CodeBinaryOperatorType.IdentityInequality,
                        customSBTypeOfRef);
            ctes = new CodeThrowExceptionStatement(
                new CodeObjectCreateExpression(
                    new CodeTypeReference(typeof(System.ArgumentException)),
                    new CodeMethodInvokeExpression(
                        new CodeTypeReferenceExpression(CodeDomHelperObjects.stringTypeRef),
                        MethodNameConstants.FormatMethod,
                        CodeDomHelperObjects.cultureInfoCurrent,
                        new CodePrimitiveExpression("Invalid type for binding. Expected type: {0}. Type passed in: {1}."),
                        new CodeMethodReferenceExpression(
                            customSBTypeOfRef,
                            PropertyNameConstants.AssemblyQualifiedNameProperty),
                        new CodeMethodReferenceExpression(
                            cmie,
                            PropertyNameConstants.AssemblyQualifiedNameProperty))));
            ccs = new CodeConditionStatement(cboe, ctes);
            onApplyCfgMethod.Statements.Add(ccs);

            string varInstanceName = Helpers.TurnFirstCharLower(standardBindingType.Name);
            CodeVariableDeclarationStatement cvds = new CodeVariableDeclarationStatement(
                                                        customSBTypeRef,
                                                        varInstanceName,
                                                        new CodeCastExpression(
                                                            standardBindingType.Name,
                                                            CodeDomHelperObjects.bindingVarRef));
            onApplyCfgMethod.Statements.Add(cvds);

            foreach (PropertyNameType nameType in propertiesGenerated)
            {
                CodeAssignStatement assignment = new CodeAssignStatement(
                                                    new CodeFieldReferenceExpression(
                                                        new CodeVariableReferenceExpression(varInstanceName),
                                                        nameType.propertyName),
                                                    new CodeFieldReferenceExpression(
                                                        CodeDomHelperObjects.ThisRef,
                                                        nameType.propertyName));
                onApplyCfgMethod.Statements.Add(assignment);
            }
            customSBConfigSectionClass.Members.Add(onApplyCfgMethod);
        }


        internal CodeCompileUnit BuildCodeGraph(out ArrayList otherGeneratedFiles)
        {
            compileUnit = new CodeCompileUnit();
            otherGeneratedFiles = new ArrayList();

            CodeNamespace nameSpace = EmitNamespaceAndUsings();
            compileUnit.Namespaces.Add(nameSpace);
            CodeTypeDeclaration customSBConfigSectionClass = EmitClass();
            nameSpace.Types.Add(customSBConfigSectionClass);

            EmitConstructors(ref customSBConfigSectionClass);
            EmitBindingElementTypeProperty(ref customSBConfigSectionClass);

            // emit only properties defined on this custom StandardBinding class; 
            // dont walk up the chain
            CodeGenHelperMethods.EmitReflectedProperties(ref customSBConfigSectionClass, 
                                                         standardBindingType, 
                                                         out propertiesGenerated, 
                                                         true, constantsClassName, defaultValuesClassName);
            EmitInitializeFromMethod(ref customSBConfigSectionClass);
            EmitOnApplyConfigurationMethod(ref customSBConfigSectionClass);
            CodeGenHelperMethods.EmitPropertiesProperty(ref customSBConfigSectionClass, propertiesGenerated, constantsClassName, defaultValuesClassName);

            // also emit the constants used in these properties that we just emitted
            otherGeneratedFiles.Add(GenerateHelperClass(constantsClassName));

            // also emit a class that can hold default values of all the properties that the user would fill in
            otherGeneratedFiles.Add(GenerateHelperClass(defaultValuesClassName));

            // also emit the custom Standard Binding section class
            otherGeneratedFiles.Add(GenerateHelperClass(generatedCollectionElementClassName));

            return compileUnit;
        }

        string GenerateHelperClass(string className)
        {
            string indent = "    ";

            string srcFile = className + "." + provider.FileExtension;
            using (StreamWriter sw = new StreamWriter(srcFile, false))
            {
                CodeCompileUnit ccu = new CodeCompileUnit();
                CodeNamespace ns = EmitNamespaceAndUsings();
                ccu.Namespaces.Add(ns);
                CodeTypeDeclaration ctd = null;
                if (className.Equals(defaultValuesClassName))
                {
                    ctd = CodeGenHelperMethods.EmitDefaultValuesClass(propertiesGenerated, className);
                }
                else if (className.Equals(constantsClassName))
                {
                    ctd = CodeGenHelperMethods.EmitConstantStringsClass(propertiesGenerated, className);
                }
                else if (className.Equals(generatedCollectionElementClassName))
                {
                    ctd = EmitCollectionElementClass();
                }
                else
                {
                    Console.WriteLine("Ignoring class: " + className);
                }
                ns.Types.Add(ctd);

                using (IndentedTextWriter itw = new IndentedTextWriter(sw, indent))
                {
                    provider.GenerateCodeFromCompileUnit(ccu, itw, new CodeGeneratorOptions());
                }
            }
            return srcFile;
        }
  
        internal string GeneratedElementClassName
        {
            get
            {
                return generatedElementClassName;
            }
        }

        internal string GeneratedCollectionElementClassName
        {
            get
            {
                return generatedCollectionElementClassName;
            }
        }

        internal string NamespaceOfGeneratedClass
        {
            get
            {
                return namespaceOfGenClass;
            }
        }

    }
}
