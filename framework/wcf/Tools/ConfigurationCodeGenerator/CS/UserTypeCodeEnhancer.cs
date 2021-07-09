//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Configuration;

namespace Microsoft.Samples.Tools.ConfigurationCodeGenerator
{
    class UserTypeCodeEnhancer
    {
        internal static void EmitCodeToAddIntoCustomStdBinding(Type standardBindingType, string generatedElementClassName, string generatedCollectionElementClassName, string srcFile)
        {
            CodeMemberMethod applyCfgMethodForStdBinding = new CodeMemberMethod();
            applyCfgMethodForStdBinding.Name = MethodNameConstants.ApplyConfigurationMethod;

            string paramConfigName = "configurationName";
            CodeVariableReferenceExpression paramVarRef = new CodeVariableReferenceExpression(paramConfigName);

            applyCfgMethodForStdBinding.Parameters.Add(new CodeParameterDeclarationExpression(
                                                          CodeDomHelperObjects.stringTypeRef,
                                                          paramConfigName));

            string bindingsString = "bindings";
            CodeVariableReferenceExpression bindingsVarRef = new CodeVariableReferenceExpression(bindingsString);

            string sectionString = "section";
            CodeVariableReferenceExpression sectionVarRef = new CodeVariableReferenceExpression(sectionString);

            string elementString = "element";
            CodeVariableReferenceExpression elementVarRef = new CodeVariableReferenceExpression(elementString);

            string topLevelSectionNameInConfig = "system.serviceModel/bindings/";
            string subSectionNameInConfig = Helpers.TurnFirstCharLower(standardBindingType.Name);

            CodeVariableDeclarationStatement bindingsInit = new CodeVariableDeclarationStatement(
                                                            new CodeTypeReference(TypeNameConstants.BindingsSection),
                                                            bindingsString,
                                                            new CodeCastExpression(TypeNameConstants.BindingsSection,
                                                                new CodeMethodInvokeExpression(
                                                                    new CodeTypeReferenceExpression(TypeNameConstants.ConfigurationManager),
                                                                    MethodNameConstants.GetSectionMethod,
                                                                    new CodePrimitiveExpression(topLevelSectionNameInConfig))));
            applyCfgMethodForStdBinding.Statements.Add(bindingsInit);

            CodeVariableDeclarationStatement sectionInit = new CodeVariableDeclarationStatement(
                                                            new CodeTypeReference(generatedCollectionElementClassName),
                                                            sectionString,
                                                            new CodeCastExpression(generatedCollectionElementClassName,
                                                                new CodeArrayIndexerExpression(
                                                                    bindingsVarRef,
                                                                    new CodePrimitiveExpression(subSectionNameInConfig))));
            applyCfgMethodForStdBinding.Statements.Add(sectionInit);

            CodeVariableDeclarationStatement elementInit = new CodeVariableDeclarationStatement(
                                                new CodeTypeReference(generatedElementClassName),
                                                elementString,
                                                new CodeArrayIndexerExpression(
                                                    new CodeFieldReferenceExpression(
                                                        sectionVarRef,
                                                        PropertyNameConstants.BindingsProperty),
                                                    paramVarRef));
            applyCfgMethodForStdBinding.Statements.Add(elementInit);

            CodeBinaryOperatorExpression cboe = new CodeBinaryOperatorExpression(
                                                    elementVarRef,
                                                    CodeBinaryOperatorType.IdentityEquality,
                                                    CodeDomHelperObjects.NullRef);

            CodeThrowExceptionStatement ctes = new CodeThrowExceptionStatement(
                        new CodeObjectCreateExpression(
                            new CodeTypeReference(typeof(ConfigurationErrorsException)),
                            new CodeMethodInvokeExpression(
                                new CodeTypeReferenceExpression(CodeDomHelperObjects.stringTypeRef),
                                MethodNameConstants.FormatMethod,
                                CodeDomHelperObjects.cultureInfoCurrent,
                                new CodePrimitiveExpression("There is no binding named {0} at {1}."),
                                paramVarRef,
                                new CodePropertyReferenceExpression(
                                    sectionVarRef, PropertyNameConstants.BindingNameProperty))));
            CodeMethodInvokeExpression cmie = new CodeMethodInvokeExpression(
                                                            elementVarRef,
                                                            MethodNameConstants.ApplyConfigurationMethod,
                                                            CodeDomHelperObjects.ThisRef);
            CodeStatement[] trueStatements = { ctes };
            CodeStatement[] falseStatements = { new CodeExpressionStatement(cmie) };
            CodeConditionStatement ccs = new CodeConditionStatement(cboe, trueStatements, falseStatements);
            applyCfgMethodForStdBinding.Statements.Add(ccs);

            string indent = "    ";
            CodeDomProvider provider = new Microsoft.CSharp.CSharpCodeProvider();

            using (System.IO.StreamWriter sbSW = new System.IO.StreamWriter(srcFile, false))
            {
                using (IndentedTextWriter sbTW = new IndentedTextWriter(sbSW, indent))
                {
                    provider.GenerateCodeFromMember(
                                    applyCfgMethodForStdBinding,
                                    sbTW,
                                    new CodeGeneratorOptions());
                }
            }
        }
    }
}
