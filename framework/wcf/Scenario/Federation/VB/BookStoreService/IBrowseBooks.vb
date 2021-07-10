' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System

Imports System.Collections.Generic

Imports System.ServiceModel

Namespace Microsoft.Samples.Federation

    <ServiceContract()> _
    Public Interface IBrowseBooks

        ''' <summary>
        ''' Allows callers to get a list of books the BookStore service sells
        ''' </summary>
        ''' <returns>A list of book titles</returns>
        <OperationContract()> _
        Function BrowseBooks() As List(Of String)

    End Interface

End Namespace

