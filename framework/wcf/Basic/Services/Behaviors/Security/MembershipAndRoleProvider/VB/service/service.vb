' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.Security.Permissions
Imports System.Security.Principal
Imports System.ServiceModel
Imports System.ServiceModel.Activation
Imports System.Threading
Imports System.Web.Security

Namespace Microsoft.Samples.MembershipAndRoleProvider

    ' Define a service contract.
    <ServiceContract([Namespace]:="http://Microsoft.Samples.MembershipAndRoleProvider")> _
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
    Public Class CalculatorService
        Implements ICalculator

        ' Allows all Users to call the Add method
        <PrincipalPermission(SecurityAction.Demand, Role:="Users")> _
        Public Function Add(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Add

            Dim result As Double = n1 + n2
            Return result

        End Function

        ' Allows all Users to call the Subtract method
        <PrincipalPermission(SecurityAction.Demand, Role:="Users")> _
        Public Function Subtract(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Subtract

            Dim result As Double = n1 - n2
            Return result

        End Function

        ' Only allow Registered Users to call the Multiply method
        <PrincipalPermission(SecurityAction.Demand, Role:="Registered Users")> _
        Public Function Multiply(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Multiply

            Dim result As Double = n1 * n2
            Return result

        End Function

        ' Only allow Super Users to call the Divide method
        <PrincipalPermission(SecurityAction.Demand, Role:="Super Users")> _
        Public Function Divide(ByVal n1 As Double, ByVal n2 As Double) As Double Implements ICalculator.Divide

            Dim result As Double = n1 / n2
            Return result

        End Function

    End Class

    Public Class CalculatorServiceHostFactory
        Inherits ServiceHostFactoryBase

        Public Overloads Overrides Function CreateServiceHost(ByVal constructorString As String, ByVal baseAddresses As Uri()) As ServiceHostBase

            Return New CalculatorServiceHost(baseAddresses)

        End Function

    End Class

    Class CalculatorServiceHost
        Inherits ServiceHost

#Region "CalculatorServiceHost Constructor"
        ''' <summary>
        ''' Constructs a CalculatorServiceHost. Calls into SetupUsersAndroles to 
        ''' set up the user and roles that the CalculatorService allows
        ''' </summary>
        Public Sub New(ByVal ParamArray addresses As Uri())

            MyBase.New(GetType(CalculatorService), addresses)
            SetupUsersAndRoles()

        End Sub
#End Region

        ''' <summary>
        ''' Sets up the user and roles that the CalculatorService allows
        ''' </summary>
        Friend Shared Sub SetupUsersAndRoles()

            ' Create some arrays for membership and role data
            Dim users() As String = New String(2) {"Alice", "Bob", "Charlie"}
            Dim emails() As String = New String(2) {"alice@example.org", "bob@example.org", "charlie@example.org"}
            Dim passwords() As String = New String(2) {"ecilA-123", "treboR-456", "eilrahC-789"}
            Dim uroles() As String = New String(2) {"Super Users", "Registered Users", "Users"}

            ' Clear out existing user information and add fresh user information
            For i As Integer = 0 To emails.Length - 1
                If Membership.GetUserNameByEmail(emails(i)) IsNot Nothing Then
                    Membership.DeleteUser(users(i), True)
                End If

                Membership.CreateUser(users(i), passwords(i), emails(i))

            Next

            For i As Integer = 0 To uroles.Length - 1

                ' Clear out existing role information and add fresh role information
                ' This puts Alice, Bob and Charlie in the Users Role, Alice and Bob 
                ' in the Registered Users Role and just Alice in the Super Users Role.
                If Roles.RoleExists(uroles(i)) Then

                    For Each u As String In Roles.GetUsersInRole(uroles(i))
                        Roles.RemoveUserFromRole(u, uroles(i))
                    Next

                    Roles.DeleteRole(uroles(i))

                End If

                Roles.CreateRole(uroles(i))

                Dim userstoadd() As String = New String(i) {}

                For j As Integer = 0 To userstoadd.Length - 1
                    userstoadd(j) = users(j)
                Next

                Roles.AddUsersToRole(userstoadd, uroles(i))

            Next

        End Sub

    End Class

End Namespace
