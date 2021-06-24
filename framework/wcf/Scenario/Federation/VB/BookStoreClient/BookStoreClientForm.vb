' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System

Imports System.Globalization

Imports System.ServiceModel
Imports System.ServiceModel.Channels

Imports System.Windows.Forms
Imports Microsoft.VisualBasic

Namespace Microsoft.Samples.Federation

    Public Class BookStoreClientForm

#Region "Browse Button Event Handler"
        ''' <summary>
        '''  Handles the operation of browsing the available books
        ''' </summary>
        Private Sub btnBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowse.Click

            ' refresh list view
            lstBooks.Items.Clear()
            ' create proxy object
            Dim client As New BrowseBooksClient()
            Try

                ' call the browseBooks() method
                Dim books As String() = client.BrowseBooks()
                For i As Integer = 0 To books.Length - 1

                    ' load results in list view
                    lstBooks.Items.Add(books(i))

                Next
                MsgBox("Books Loaded for Browsing", MsgBoxStyle.OkOnly, "Bookstore Client")

                If Not btnBuy.Enabled Then

                    btnBuy.Enabled = True

                End If

                client.Close()
            Catch generatedExceptionName As EndpointNotFoundException
                MsgBox("Error while loading books for browsing. Make sure the BookStore Service, BookStore STS, and HomeRealm STS have been started", MsgBoxStyle.OkOnly, "Bookstore Client")
            Catch ex As CommunicationException
                client.Abort()
                MsgBox([String].Format("Communication error while loading books for browsing: {0}", ex), MsgBoxStyle.OkOnly, "Bookstore Client")
            Catch ex As TimeoutException
                client.Abort()
                MsgBox([String].Format("Timeout error while loading books for browsing: {0}", ex), MsgBoxStyle.OkOnly, "Bookstore Client")
            Catch ex As Exception
                client.Abort()
                MsgBox([String].Format("Unexpected error while loading books for browsing: {0}", ex), MsgBoxStyle.OkOnly, "Bookstore Client")
            End Try

        End Sub
#End Region

#Region "Buy Button Event Handler"
        Private Sub btnBuy_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBuy.Click
            ' check if any book is selected for purchase
            If lstBooks.SelectedItem Is Nothing Then

                MsgBox("No book selected for purchase", MsgBoxStyle.OkOnly, "BookStore Client")

            Else

                ' get the selected book ID
                Dim selectedBookItem As String = lstBooks.SelectedItem.ToString()
                Dim startPos As Integer = selectedBookItem.IndexOf("."c) + 1
                Dim endPos As Integer = selectedBookItem.IndexOf(","c)
                Dim bookName As String = selectedBookItem.Substring(startPos, endPos - startPos)
                bookName = bookName.Trim()

                Dim myBuyBookClient As New BuyBookClient()
                Try

                    ' Add the book name as a "resource" header to the endpoint address for the service
                    Dim myEndpointAddressBuilder As New EndpointAddressBuilder(myBuyBookClient.Endpoint.Address)
                    myEndpointAddressBuilder.Headers.Add(AddressHeader.CreateAddressHeader(Constants.BookNameHeaderName, Constants.BookNameHeaderNamespace, bookName))
                    myBuyBookClient.Endpoint.Address = myEndpointAddressBuilder.ToEndpointAddress()

                    ' Send the request to the service. This will result in the following steps;
                    ' 1. Request a security token from HomeRealmSTS authenticating using Windows credentials
                    ' 2. Request a security token from BookStoreSTS authenticating using token from 1.
                    ' 3. Send the BuyBook request to the BookStoreService authenticating using token from 2.
                    Dim response As String = myBuyBookClient.BuyBook("someone@microsoft.com", "One Microsoft Way, Redmond, WA 98052")
                    MsgBox(response, MsgBoxStyle.OkOnly, "BookStore Client")

                    myBuyBookClient.Close()

                Catch ex As Exception

                    myBuyBookClient.Abort()

                    ' see if a fault has been sent back
                    Dim inner As FaultException = TryCast(GetInnerException(ex), FaultException)
                    If inner IsNot Nothing Then

                        Dim fault As MessageFault = inner.CreateMessageFault()
                        MsgBox([String].Format("The server sent back a fault: {0}", fault.Reason.GetMatchingTranslation(CultureInfo.CurrentCulture).Text), MsgBoxStyle.OkOnly)

                    Else

                        MsgBox([String].Format("Exception while trying to purchase the selected book: {0}", ex), MsgBoxStyle.OkOnly, "BookStore Client")

                    End If

                End Try

            End If

        End Sub
#End Region

#Region "Helper method to get inner exception"
        Private Shared Function GetInnerException(ByVal ex As Exception) As Exception

            If ex Is Nothing Then

                Throw New ArgumentNullException("ex")

            End If

            Dim innerEx As Exception = ex
            While innerEx.InnerException IsNot Nothing

                innerEx = innerEx.InnerException

            End While

            Return innerEx

        End Function
#End Region

    End Class

End Namespace
