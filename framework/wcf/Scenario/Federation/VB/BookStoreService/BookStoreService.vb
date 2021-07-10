' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System

Imports System.Collections.Generic

Imports System.IdentityModel.Claims

Imports System.IO

Imports System.Security.Principal

Imports System.ServiceModel

Namespace Microsoft.Samples.Federation

    <ServiceBehavior(IncludeExceptionDetailInFaults:=True)> _
    Public Class BookStoreService
        Implements IBrowseBooks
        Implements IBuyBook

#Region "BookStoreService Constructor"
        ''' <summary>
        ''' Sets up the BookStoreService by loading relevant Application Settings
        ''' </summary>
        Public Sub New()

        End Sub
#End Region

#Region "BrowseBooks() Implementation"
        ''' <summary>
        ''' browseBooks() service call Implementation
        ''' </summary>
        ''' <returns>List of books available for purchase in the bookstore</returns>
        Public Function BrowseBooks() As List(Of String) Implements IBrowseBooks.BrowseBooks

            ' Create an empty list of strings.
            Dim books As New List(Of String)()
            Try

                ' Create a StreamReader over the text file specified in app.config
                Using myStreamReader As New StreamReader(ServiceConstants.BookDB)

                    Dim line As String = ""
                    ' For each line in the text file...
                    line = myStreamReader.ReadLine()
                    While line IsNot Nothing

                        ' ...split the text from the text file...
                        Dim splitEntry As String() = line.Split("#"c)
                        ' ...format a string to return...
                        ' Book ID 
                        ' Book Name
                        ' Author
                        ' Price
                        Dim formattedEntry As String = [String].Format("{0}.  {1},  {2},  ${3}", splitEntry(0), splitEntry(1), splitEntry(2), splitEntry(3))
                        ' ...and add it to the list 
                        books.Add(formattedEntry)
                        line = myStreamReader.ReadLine()
                    End While
                    ' Once we've finished reading the file, return the list of strings
                    Return books

                End Using

            Catch e As Exception

                Throw New Exception([String].Format("BookStoreService: Error while loading books from DB ", e))

            End Try

        End Function
#End Region

#Region "BuyBook() Implementation"
        ''' <summary>
        ''' This function extracts a Name claim from the provided ServiceSecurityContext and 
        ''' returns the associated resource value.
        ''' </summary>
        ''' <param name="securityContext">The ServiceSecurityContext in which the Name claim should be found</param>
        ''' <returns>The resource value associated </returns>
        Private Shared Function GetNameIdentity(ByVal securityContext As ServiceSecurityContext) As String

            ' Iterate through each of the claimsets in the AuthorizationContext.
            For Each claimSet As ClaimSet In securityContext.AuthorizationContext.ClaimSets

                ' Find all the Name claims
                Dim nameClaims As IEnumerable(Of Claim) = claimSet.FindClaims(ClaimTypes.Name, Rights.PossessProperty)
                If nameClaims IsNot Nothing Then

                    ' Get the first claim 
                    Dim enumerator As IEnumerator(Of Claim) = nameClaims.GetEnumerator()
                    If enumerator.MoveNext() Then

                        ' return the resource value.
                        Return enumerator.Current.Resource.ToString()

                    End If

                End If

            Next

            ' If there are no Name claims in the AuthorizationContext, return the Name of the Anonymous Windows Identity.
            Return WindowsIdentity.GetAnonymous().Name

        End Function

        Public Function BuyBook(ByVal emailAddress As String, ByVal shipAddress As String) As String Implements IBuyBook.BuyBook

            ' get the book id from the headers
            Dim bookName As String = OperationContext.Current.IncomingMessageHeaders.GetHeader(Of String)(Constants.BookNameHeaderName, Constants.BookNameHeaderNamespace)
            If bookName Is Nothing Then

                Throw New FaultException(Of String)("The name of the book to be purchased was not specified")

            End If
            ' Get the callers name
            Dim caller As String = GetNameIdentity(ServiceSecurityContext.Current)
            Return [String].Format("{0}, the purchase of book {1} has been approved. The details of shipping date and confirmation receipt will be mailed to {2} shortly", caller, bookName, emailAddress)

        End Function
#End Region

    End Class

End Namespace

