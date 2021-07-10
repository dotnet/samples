' Copyright (c) Microsoft Corporation. All rights reserved.

Imports System.Security.Permissions
Imports System.ServiceModel

Namespace Microsoft.Samples.WS2007FederationHttpBinding
    ' Define a service contract.
    <ServiceContract([Namespace]:="http://Microsoft.Samples.WS2007FederationHttpBinding")> _
    Public Interface ICalculator
        <OperationContract()> _
        Function Add(ByVal n1 As Double, ByVal n2 As Double) As Double
        <OperationContract()> _
        Function Subtract(ByVal n1 As Double, ByVal n2 As Double) As Double
        <OperationContract()> _
        Function Multiply(ByVal n1 As Double, ByVal n2 As Double) As Double
        <OperationContract()> _
        Function Divide(ByVal n1 As Double, ByVal n2 As Double) As Double
    End Interface
End Namespace
