' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System

Imports System.Collections.Generic

Imports System.IdentityModel.Claims
Imports System.IdentityModel.Policy

Imports System.ServiceModel

Namespace Microsoft.Samples.Federation

    Public Class BuyAuthorizationManager
        Inherits ServiceAuthorizationManager

#Region "AccessCheck() override"
        ''' <summary>
        ''' Implementation of the framework level access control mechanism via ServiceAuthorizationManager
        ''' </summary>
        ''' <returns>True on success. False on failure.</returns>
        Public Overloads Overrides Function CheckAccess(ByVal operationContext As OperationContext) As Boolean

            ' BrowseBooks is always authorized, so return true
            If operationContext.IncomingMessageHeaders.Action = ServiceConstants.BrowseBooksAction Then
                Return True
            End If

            ' If the requested operation is NOT BuyBook, return false (Access Denied)
            ' The only operation we support apart from BrowseBooks is BuyBook
            If operationContext.IncomingMessageHeaders.Action <> ServiceConstants.BuyBookAction Then
                Return False
            End If

            ' Extract the AuthorizationContext from the ServiceSecurityContext
            Dim authContext As AuthorizationContext = operationContext.ServiceSecurityContext.AuthorizationContext
            ' If there are no Claims in the AuthorizationContext, return false (Access Denied)
            ' The issued token used to authenticate should contain claims 

            If authContext.ClaimSets Is Nothing Then
                Return False
            End If

            ' If there is more than one ClaimSet in the AuthorizationContext, return false (Access Denied).
            ' The issued token used to authenticate should only contain a single set of claims.
            If authContext.ClaimSets.Count <> 2 Then
                Return False
            End If

            ' Extract the single ClaimSet from the AuthorizationContext
            Dim claimsets As New List(Of ClaimSet)(authContext.ClaimSets)
            Dim myClaimSet As ClaimSet = claimsets.Find(DirectCast(AddressOf ConvertedAnonymousMethod1, Predicate(Of ClaimSet)))

            ' Is the ClaimSet was NOT issued by the BookStoreSTS then return false (Access Denied)
            ' The BookStoreService only accepts requests where the client authenticated using a token
            ' issued by the BookStoreSTS.
            If Not IssuedByBookStoreSTS(myClaimSet) Then
                Return False
            End If

            ' Find all the PurchaseAuthorized claims
            Dim purchaseAllowedClaims As IEnumerable(Of Claim) = myClaimSet.FindClaims(Constants.PurchaseAuthorizedClaim, Rights.PossessProperty)

            ' If there are no PurchaseAuthorized claims, return false (Access Denied)
            ' The BookStoreSTS issues tokens containing PurchaseAuthorized claims for all authorized requests.
            If purchaseAllowedClaims Is Nothing Then
                Return False
            End If

            ' Get the book name being purchased...
            Dim bookName As String = operationContext.IncomingMessageHeaders.GetHeader(Of String)(Constants.BookNameHeaderName, Constants.BookNameHeaderNamespace)

            ' ..and if it's null or empty, return false (Access Denied)
            If String.IsNullOrEmpty(bookName) Then
                Return False
            End If

            ' Iterate through the PurchaseAllowed claims and verify that the Resource value is 
            ' the same as the book name retrieved above
            For Each claim As Claim In purchaseAllowedClaims

                Dim authorizedBook As String = claim.Resource.ToString()
                If Not String.IsNullOrEmpty(authorizedBook) AndAlso authorizedBook.Trim().Equals(bookName.Trim(), StringComparison.OrdinalIgnoreCase) Then
                    Return True
                End If

            Next

            ' If no PurchaseAllowed claim had a resource value that matched the 
            ' book name, return false (Access Denied)
            Return False

        End Function
#End Region

#Region "IssuedByBookStoreSTS"
        ''' <summary>
        ''' Helper function to check if claims were issued by BookStoreSTS
        ''' </summary>
        ''' <returns>True on success. False on failure.</returns>
        Private Shared Function IssuedByBookStoreSTS(ByVal myClaimSet As ClaimSet) As Boolean

            ' Extract the issuer ClaimSet
            Dim issuerClaimSet As ClaimSet = myClaimSet.Issuer

            ' If the Issuer is null, return false.
            If issuerClaimSet Is Nothing Then
                Return False
            End If

            ' Find all the Thumbprint claims in the issuer ClaimSet
            Dim issuerClaims As IEnumerable(Of Claim) = issuerClaimSet.FindClaims(ClaimTypes.Thumbprint, Nothing)

            ' If there are no Thumbprint claims, return false;
            If issuerClaims Is Nothing Then
                Return False
            End If

            ' Get the enumerator for the set of Thumbprint claims...                        
            Dim issuerClaimsEnum As IEnumerator(Of Claim) = issuerClaims.GetEnumerator()

            ' ...and set issuerClaim to the first such Claim
            Dim issuerClaim As Claim = Nothing
            If issuerClaimsEnum.MoveNext() Then
                issuerClaim = issuerClaimsEnum.Current
            End If

            ' If there was no Thumbprint claim, return false;
            If issuerClaim Is Nothing Then
                Return False
            End If

            ' If, despite the above checks, the returned claim is not a Thumbprint claim, return false
            If issuerClaim.ClaimType <> ClaimTypes.Thumbprint Then
                Return False
            End If

            ' If the returned claim doesn't contain a Resource, return false
            If issuerClaim.Resource Is Nothing Then
                Return False
            End If

            ' Extract the thmubprint data from the claim
            Dim thumbprint() As Byte = DirectCast(issuerClaim.Resource, Byte())

            ' Extract the thumbprint for the BookStoreSTS.com certificate
            Dim issuerCertThumbprint() As Byte = FederationUtilities.GetCertificateThumbprint(ServiceConstants.CertStoreName, ServiceConstants.CertStoreLocation, ServiceConstants.IssuerCertDistinguishedName)

            ' If the lengths of the two thumbprints are different, return false
            If thumbprint.Length <> issuerCertThumbprint.Length Then
                Return False
            End If

            ' Check the individual bytes of the two thumbprints for equality...
            For i As Integer = 0 To thumbprint.Length - 1

                '... if any byte in the thumbprint from the claim does NOT match the corresponding
                ' byte from the thumbprint in the BookStoreSTS.com certificate, return false
                If thumbprint(i) <> issuerCertThumbprint(i) Then
                    Return False
                End If

            Next

            ' If we get through all the above checks, return true (ClaimSet was issued by BookStoreSTS.com)
            Return True

        End Function
#End Region

        Private Function ConvertedAnonymousMethod1(ByVal target As ClaimSet) As Boolean
            Dim certClaimSet As X509CertificateClaimSet = TryCast(target.Issuer, X509CertificateClaimSet)
            Return certClaimSet IsNot Nothing AndAlso certClaimSet.X509Certificate.Subject = "CN=BookStoreSTS.com"

        End Function

    End Class

End Namespace

