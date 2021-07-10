' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.Configuration
Imports System.Security.Permissions
Imports System.Security.Principal
Imports System.ServiceModel
Imports System.Threading

Namespace Microsoft.Samples.PrincipalPermissionAuthorization

    ' Define a service contract.
    <ServiceContract([Namespace]:="http://Microsoft.Samples.PrincipalPermissionAuthorization")> _
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

    ' Service class which implements the service contract.
    ' Added PrincipalPermission attributes to authorize administrators to access each operation
    Public Class CalculatorService
        Implements ICalculator

        <PrincipalPermission(SecurityAction.Demand, Role:="Builtin\Administrators")> _
        Public Function Add(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Add

            Dim result As Double = n1 + n2
            Return result

        End Function

        <PrincipalPermission(SecurityAction.Demand, Role:="Builtin\Administrators")> _
        Public Function Subtract(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Subtract

            Dim result As Double = n1 - n2
            Return result

        End Function

        <PrincipalPermission(SecurityAction.Demand, Role:="Builtin\Administrators")> _
        Public Function Multiply(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Multiply

            Dim result As Double = n1 * n2
            Return result

        End Function

        <PrincipalPermission(SecurityAction.Demand, Role:="Builtin\Administrators")> _
        Public Function Divide(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Divide

            Dim result As Double = n1 / n2
            Return result

        End Function

    End Class

End Namespace

