' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System

Imports System.ServiceModel

Namespace Microsoft.Samples.Federation

    <ServiceContract()> _
    Public Interface IBuyBook

        ''' <summary>
        ''' The book ID is not part of the OperationContract because it is
        ''' included as an EndpointAddress header. This is in order to do resource based
        ''' authorization at the service's SecurityTokenService
        ''' </summary>
        ''' <param name="emailAddress">The e-mail address any shipping information should be sent to</param>
        ''' <param name="shipAddress">The shipping address the book being purchased should be sent to</param>
        ''' <returns></returns>
        <OperationContract()> _
        Function BuyBook(ByVal emailAddress As String, ByVal shipAddress As String) As String

    End Interface

End Namespace

