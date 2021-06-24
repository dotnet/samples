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
    // This will generate the config section for the BindingElement
    class BindingElementExtensionSectionGenerator
    {
        CodeDomProvider provider;
        CodeCompileUnit compileUnit;
        Type bindingElementType;
        Assembly userAssembly;

        // The name of the class we will generate
        string generatedClassName;

        // The namespace of the generated class
        string namespaceOfGenClass;

        // A class with some constants that we will generate
        string constantsClassName;

        // A class with default values of properties that we will generate
        string defaultValuesClassName;

        // Name-Type of all public props we will generate for the custom BindingElement
        PropertyNameType[] propertiesGenerated;

        string customBEVarInstance;

        // CodeDom objects
        CodeTypeReference customBETypeRef;
        CodeTypeOfExpression customBETypeOfRef;
        CodeArgumentReferenceExpression customBEArgRef;
        CodeVariableDeclarationStatement customBENewVarAssignRef;
        CodeParameterDeclarationExpression bindingElementMethodParamRef;

        internal BindingElementExtensionSectionGenerator(Type bindingElementType, Assembly userAssembly, CodeDomProvider provider)
        {
            this.bindingElementType = bindingElementType;
            this.userAssembly = userAssembly;
            this.provider = provider;

            string typePrefix = bindingElementType.Name.Substring(0, bindingElementType.Name.IndexOf(TypeNameConstants.BindingElement));
            this.generatedClassName = typePrefix + Constants.ElementSuffix;
            this.constantsClassName = bindingElementType.Name.Substring(0, bindingElementType.Name.IndexOf(TypeNameConstants.BindingElement)) + Constants.ConfigurationStrings;
            this.defaultValuesClassName = bindingElementType.Name.Substring(0, bindingElementType.Name.IndexOf(TypeNameConstants.BindingElement)) + Constants.Defaults;

            this.customBEVarInstance = Helpers.TurnFirstCharLower(bindingElementType.Name);
            customBEArgRef = new CodeArgumentReferenceExpression(customBEVarInstance);

            this.customBETypeRef = new CodeTypeReference(bindingElementType.Name);
            this.customBETypeOfRef = new CodeTypeOfExpression(customBETypeRef);
            this.customBENewVarAssignRef = new CodeVariableDeclarationStatement(
                                                customBETypeRef,
                                                customBEVarInstance,
                                                new CodeObjectCreateExpression(customBETypeRef));
            this.bindingElementMethodParamRef = new CodeParameterDeclarationExpression(
                                                    CodeDomHelperObjects.bindingElementTypeRef,
                                                    Constants.bindingElementParamName);

        }

        CodeNamespace EmitNamespaceAndUsings()
        {
            // same namespace as the namespace of the Custom BindingElement class
            CodeNamespace nameSpace = new CodeNamespace(bindingElementType.Namespace);
            namespaceOfGenClass = nameSpace.Name;

            nameSpace.Imports.Add(CodeDomHelperObjects.systemNS);
            nameSpace.Imports.Add(CodeDomHelperObjects.systemConfigNS);
            nameSpace.Imports.Add(CodeDomHelperObjects.systemSMNS);
            nameSpace.Imports.Add(CodeDomHelperObjects.systemSMChannelsNS);
            nameSpace.Imports.Add(CodeDomHelperObjects.systemSMConfigNS);

            return nameSpace;
        }

        CodeTypeDeclaration EmitClass()
        {
            CodeTypeDeclaration customBESectionClass = new CodeTypeDeclaration(generatedClassName);
            customBESectionClass.BaseTypes.Add(new CodeTypeReference(TypeNameConstants.BindingElementExtensionSection));
            return customBESectionClass;
        }

        void EmitConstructors(ref CodeTypeDeclaration customBESectionClass)
        {
            CodeConstructor defaultCons = new CodeConstructor();
            defaultCons.Attributes = CodeDomHelperObjects.Public;
            customBESectionClass.Members.Add(defaultCons);
        }

        void EmitBindingElementTypeProperty(ref CodeTypeDeclaration customBESectionClass)
        {
            CodeMemberProperty bindingElementTypeProperty = new CodeMemberProperty();
            bindingElementTypeProperty.Name = PropertyNameConstants.BindingElementTypeProperty;
            bindingElementTypeProperty.Attributes = CodeDomHelperObjects.PublicOverride;
            bindingElementTypeProperty.HasSet = false;
            bindingElementTypeProperty.HasGet = true;
            bindingElementTypeProperty.Type = CodeDomHelperObjects.typeTypeRef;
            bindingElementTypeProperty.GetStatements.Add(new CodeMethodReturnStatement(customBETypeOfRef));
            customBESectionClass.Members.Add(bindingElementTypeProperty);
        }

        void EmitCreateBindingElementMethod(ref CodeTypeDeclaration customBESectionClass)
        {
            CodeMemberMethod cbeMethod = new CodeMemberMethod();
            cbeMethod.Name = MethodNameConstants.CreateBindingElementMethod;
            cbeMethod.ReturnType = CodeDomHelperObjects.bindingElementTypeRef;
            cbeMethod.Attributes = CodeDomHelperObjects.ProtectedOverride;
            cbeMethod.Statements.Add(customBENewVarAssignRef);
            CodeMethodInvokeExpression invokeApplyConfig = new CodeMethodInvokeExpression(
                                                                    CodeDomHelperObjects.ThisRef,
                                                                    MethodNameConstants.ApplyConfigurationMethod,
                                                                    customBEArgRef);
            cbeMethod.Statements.Add(invokeApplyConfig);
            cbeMethod.Statements.Add(new CodeMethodReturnStatement(customBEArgRef));
            customBESectionClass.Members.Add(cbeMethod);
        }

        void EmitApplyConfigurationMethod(ref CodeTypeDeclaration customBESectionClass)
        {
            CodeMemberMethod applyCfgMethod = new CodeMemberMethod();
            applyCfgMethod.Name = MethodNameConstants.ApplyConfigurationMethod;
            applyCfgMethod.Parameters.Add(bindingElementMethodParamRef);
            applyCfgMethod.Attributes = CodeDomHelperObjects.PublicOverride;
            CodeMethodInvokeExpression invokeBase = new CodeMethodInvokeExpression(
                                                        CodeDomHelperObjects.BaseRef,
                                                        MethodNameConstants.ApplyConfigurationMethod,
                                                        CodeDomHelperObjects.bindingElementArgRef);
            applyCfgMethod.Statements.Add(invokeBase);
            string varInstanceName = Helpers.TurnFirstCharLower(bindingElementType.Name);
            CodeVariableDeclarationStatement cvds = new CodeVariableDeclarationStatement(
                                                        customBETypeRef,
                                                        varInstanceName,
                                                        new CodeCastExpression(
                                                            bindingElementType.Name,
                                                            CodeDomHelperObjects.bindingElementVarRef));
            applyCfgMethod.Statements.Add(cvds);

            foreach (PropertyNameType nameType in propertiesGenerated)
            {
                CodeAssignStatement assignment = new CodeAssignStatement(
                                                    new CodeFieldReferenceExpression(
                                                        new CodeVariableReferenceExpression(varInstanceName),
                                                        nameType.propertyName),
                                                    new CodeFieldReferenceExpression(
                                                        CodeDomHelperObjects.ThisRef,
                                                        nameType.propertyName));
                applyCfgMethod.Statements.Add(assignment);
            }
            customBESectionClass.Members.Add(applyCfgMethod);
        }

        void EmitInitializeFromMethod(ref CodeTypeDeclaration customBESectionClass)
        {
            CodeMemberMethod initFromMethod = new CodeMemberMethod();
            initFromMethod.Name = MethodNameConstants.InitializeFromMethod;
            initFromMethod.Parameters.Add(bindingElementMethodParamRef);
            initFromMethod.Attributes = CodeDomHelperObjects.ProtectedOverride;
            CodeMethodInvokeExpression invokeBase = new CodeMethodInvokeExpression(
                                                        CodeDomHelperObjects.BaseRef,
                                                        MethodNameConstants.InitializeFromMethod,
                                                        CodeDomHelperObjects.bindingElementArgRef);
            initFromMethod.Statements.Add(invokeBase);
            string varInstanceName = Helpers.TurnFirstCharLower(bindingElementType.Name);
            CodeVariableDeclarationStatement cvds = new CodeVariableDeclarationStatement(
                                                            customBETypeRef,
                                                            varInstanceName,
                                                            new CodeCastExpression(
                                                                bindingElementType.Name,
                                                                CodeDomHelperObjects.bindingElementVarRef));
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
            customBESectionClass.Members.Add(initFromMethod);
        }

        void EmitCopyFromMethod(ref CodeTypeDeclaration customBESectionClass)
        {
            CodeMemberMethod copyFromMethod = new CodeMemberMethod();
            copyFromMethod.Name = MethodNameConstants.CopyFromMethod;
            string varFrom = "from";
            copyFromMethod.Parameters.Add(new CodeParameterDeclarationExpression(
                                            new CodeTypeReference(TypeNameConstants.ServiceModelExtensionSection),
                                            varFrom));
            copyFromMethod.Attributes = CodeDomHelperObjects.PublicOverride;
            CodeMethodInvokeExpression invokeBase = new CodeMethodInvokeExpression(
                                                            CodeDomHelperObjects.BaseRef,
                                                            MethodNameConstants.CopyFromMethod,
                                                            new CodeArgumentReferenceExpression(varFrom));
            copyFromMethod.Statements.Add(invokeBase);

            string varSource = "source";
            CodeVariableDeclarationStatement cvds = new CodeVariableDeclarationStatement(
                                                        new CodeTypeReference(customBESectionClass.Name),
                                                        varSource,
                                                        new CodeCastExpression(
                                                            customBESectionClass.Name,
                                                            new CodeVariableReferenceExpression(varFrom)));
            copyFromMethod.Statements.Add(cvds);

            foreach (PropertyNameType nameType in propertiesGenerated)
            {
                CodeAssignStatement assignment = new CodeAssignStatement(
                                                    new CodeFieldReferenceExpression(
                                                        CodeDomHelperObjects.ThisRef,
                                                        nameType.propertyName),
                                                    new CodeFieldReferenceExpression(
                                                        new CodeVariableReferenceExpression(varSource),
                                                        nameType.propertyName)
                                                    );
                copyFromMethod.Statements.Add(assignment);
            }
            customBESectionClass.Members.Add(copyFromMethod);
        }

        internal CodeCompileUnit BuildCodeGraph(out ArrayList otherGeneratedFiles)
        {
            compileUnit = new CodeCompileUnit();
            otherGeneratedFiles = new ArrayList();

            CodeNamespace nameSpace = EmitNamespaceAndUsings();
            compileUnit.Namespaces.Add(nameSpace);
            CodeTypeDeclaration customBESectionClass = EmitClass();
            nameSpace.Types.Add(customBESectionClass);

            EmitConstructors(ref customBESectionClass);
            EmitBindingElementTypeProperty(ref customBESectionClass);
            EmitCreateBindingElementMethod(ref customBESectionClass);

            CodeGenHelperMethods.EmitReflectedProperties(ref customBESectionClass, bindingElementType, out propertiesGenerated, constantsClassName, defaultValuesClassName);

            EmitApplyConfigurationMethod(ref customBESectionClass);
            EmitInitializeFromMethod(ref customBESectionClass);
            EmitCopyFromMethod(ref customBESectionClass);
            CodeGenHelperMethods.EmitPropertiesProperty(ref customBESectionClass, propertiesGenerated, constantsClassName, defaultValuesClassName);

            // also emit the constants used in these properties that we just emitted
            otherGeneratedFiles.Add(GenerateHelperClass(constantsClassName));

            // also emit a class that can hold default values of all the properties that the user would fill in
            otherGeneratedFiles.Add(GenerateHelperClass(defaultValuesClassName));
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
                else
                {
                    Console.WriteLine("Don't know how to generate this helper class, ignoring: " + className);
                }
                ns.Types.Add(ctd);

                using (IndentedTextWriter itw = new IndentedTextWriter(sw, indent))
                {
                    provider.GenerateCodeFromCompileUnit(ccu, itw, new CodeGeneratorOptions());
                }
            }
            return srcFile;
        }
  
        internal string GeneratedClassName
        {
            get
            {
                return generatedClassName;
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
