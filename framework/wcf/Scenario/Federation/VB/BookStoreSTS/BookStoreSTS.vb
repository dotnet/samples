' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System

Imports System.Collections.Generic
Imports System.Collections.ObjectModel

Imports System.IdentityModel.Claims
Imports System.IdentityModel.Policy
Imports System.IdentityModel.Tokens

Imports System.IO

Imports System.ServiceModel
Imports System.Security.Permissions

Namespace Microsoft.Samples.Federation

    <ServiceBehavior(IncludeExceptionDetailInFaults:=True)> _
    Public Class BookStoreSTS
        Inherits SecurityTokenService

        ''' <summary>
        ''' Sets up the BookStoreSTS by loading relevant Application Settings
        ''' </summary>
        Public Sub New()

            MyBase.New(ServiceConstants.SecurityTokenServiceName, FederationUtilities.GetX509TokenFromCert(ServiceConstants.CertStoreName, ServiceConstants.CertStoreLocation, ServiceConstants.CertDistinguishedName), FederationUtilities.GetX509TokenFromCert(ServiceConstants.CertStoreName, ServiceConstants.CertStoreLocation, ServiceConstants.TargetDistinguishedName))

        End Sub

        ''' <summary>
        ''' Overrides the SetUpClaims from the SecurityTokenService Base Class
        ''' Checks if the caller can purchase the book specified in the RST and if so
        ''' issues a purchase authorized claim
        ''' </summary>
        Protected Overloads Overrides Function GetIssuedClaims(ByVal requestSecurityToken As RequestSecurityToken) As Collection(Of SamlAttribute)

            Dim rstAppliesTo As EndpointAddress = requestSecurityToken.AppliesTo

            If rstAppliesTo Is Nothing Then

                Throw New InvalidOperationException("No AppliesTo EndpointAddress in RequestSecurityToken")

            End If

            Dim bookName As String = rstAppliesTo.Headers.FindHeader(Constants.BookNameHeaderName, Constants.BookNameHeaderNamespace).GetValue(Of String)()
            If String.IsNullOrEmpty(bookName) Then
                Throw New FaultException("The book name was not specified in the RequestSecurityToken")
            End If

            EnsurePurchaseLimitSufficient(bookName)

            Dim samlAttributes As New Collection(Of SamlAttribute)()

            For Each claimSet As ClaimSet In ServiceSecurityContext.Current.AuthorizationContext.ClaimSets

                ' Copy Name claims from the incoming credentials into the set of claims we're going to issue
                Dim nameClaims As IEnumerable(Of Claim) = claimSet.FindClaims(ClaimTypes.Name, Rights.PossessProperty)
                If nameClaims IsNot Nothing Then

                    For Each nameClaim As Claim In nameClaims

                        samlAttributes.Add(New SamlAttribute(nameClaim))

                    Next

                End If

            Next

            ' add a purchase authorized claim
            samlAttributes.Add(New SamlAttribute(New Claim(Constants.PurchaseAuthorizedClaim, bookName, Rights.PossessProperty)))
            Return samlAttributes

        End Function

        ''' <summary>
        ''' This method adds the audience uri restriction condition to the SAML assetion.
        ''' </summary>
        ''' <param name="samlConditions">The saml condition collection where the audience uri restriction condition will be added.</param>
        Public Overloads Overrides Sub AddAudienceRestrictionCondition(ByVal samlConditions As SamlConditions)
            samlConditions.Conditions.Add(New SamlAudienceRestrictionCondition(New Uri() {New Uri(Constants.BookStoreServiceAudienceUri)}))
        End Sub

#Region "Helper Methods"
        ''' <summary>
        ''' Wrapper for the Application level check performed at the BookStoreSTS for 
        ''' the existence of required purchase limit 
        ''' </summary>
        Private Shared Sub EnsurePurchaseLimitSufficient(ByVal bookName As String)

            If Not CheckIfPurchaseLimitMet(bookName) Then

                Throw New FaultException([String].Format("Purchase limit not sufficient to purchase '{0}'", bookName))

            End If

        End Sub

        ''' <summary>
        ''' Helper method to get book price from the Books Database
        ''' </summary>
        ''' <param name="bookID">ID of the book intended for purchase</param>
        ''' <returns>Price of the book with the given ID</returns>
        Private Shared Function GetBookPrice(ByVal bookName As String) As Double

            Using myStreamReader As New StreamReader(ServiceConstants.BookDB)

                Dim line As String = myStreamReader.ReadLine()
                While (line IsNot Nothing)

                    Dim splitEntry() As String = line.Split("#"c)
                    If splitEntry(1).Trim().Equals(bookName.Trim(), StringComparison.OrdinalIgnoreCase) Then

                        Return [Double].Parse(splitEntry(3))

                    End If

                    line = myStreamReader.ReadLine()

                End While

                ' invalid bookName - throw
                Throw New FaultException("Invalid Book Name " & bookName)

            End Using

        End Function

        ''' <summary>
        ''' Application level check for claims at the BookStoreSTS
        ''' </summary>
        ''' <param name="bookID">ID of the book intended for purchase</param>
        ''' <returns>True on success. False on failure.</returns>
        Private Shared Function CheckIfPurchaseLimitMet(ByVal bookID As String) As Boolean

            ' Extract the AuthorizationContext from the ServiceSecurityContext
            Dim authContext As AuthorizationContext = OperationContext.Current.ServiceSecurityContext.AuthorizationContext

            ' If there are no Claims in the AuthorizationContext, return false
            ' The issued token used to authenticate should contain claims 
            If authContext.ClaimSets Is Nothing Then
                Return False
            End If

            ' If there is more than two ClaimSets in the AuthorizationContext, return false
            ' The issued token used to authenticate should only contain two sets of claims.
            If authContext.ClaimSets.Count <> 2 Then
                Return False
            End If

            Dim claimsets As New List(Of ClaimSet)(authContext.ClaimSets)
            Dim myClaimSet As ClaimSet = claimsets.Find(AddressOf ConvertedAnonymousMethod1)

            ' Is the ClaimSet was NOT issued by the HomeRealmSTS then return false
            ' The BookStoreSTS only accepts requests where the client authenticated using a token
            ' issued by the HomeRealmSTS.
            If Not IssuedByHomeRealmSTS(myClaimSet) Then
                Return False
            End If

            ' Find all the PurchaseLimit claims
            Dim purchaseLimitClaims As IEnumerable(Of Claim) = myClaimSet.FindClaims(Constants.PurchaseLimitClaim, Rights.PossessProperty)

            ' If there are no PurchaseLimit claims, return false
            ' The HomeRealmSTS issues tokens containing PurchaseLimit claims for all authorized requests.
            If purchaseLimitClaims Is Nothing Then
                Return False
            End If

            ' Get the price of the book being purchased...
            Dim bookPrice As Double = GetBookPrice(bookID)

            ' Iterate through the PurchaseLimit claims and verify that the Resource value is 
            ' greater than or equal to the price of the book being purchased
            For Each c As Claim In purchaseLimitClaims

                Dim purchaseLimit As Double = [Double].Parse(c.Resource.ToString())
                If purchaseLimit >= bookPrice Then
                    Return True
                End If

            Next

            ' If no PurchaseLimit claim had a resource value that was greater than or equal
            ' to the price of the book being purchased, return false
            Return False

        End Function

        ''' <summary>
        ''' Helper function to check if SAML Token was issued by HomeRealmSTS
        ''' </summary>
        ''' <returns>True on success. False on failure.</returns>
        Private Shared Function IssuedByHomeRealmSTS(ByVal myClaimSet As ClaimSet) As Boolean

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
            Dim issuerThumbprint() As Byte = DirectCast(issuerClaim.Resource, Byte())

            ' Extract the thumbprint for the HomeRealmSTS.com certificate
            Dim certThumbprint() As Byte = FederationUtilities.GetCertificateThumbprint(ServiceConstants.CertStoreName, ServiceConstants.CertStoreLocation, ServiceConstants.IssuerDistinguishedName)

            ' If the lengths of the two thumbprints are different, return false
            If issuerThumbprint.Length <> certThumbprint.Length Then
                Return False
            End If

            ' Check the individual bytes of the two thumbprints for equality...
            For i As Integer = 0 To issuerThumbprint.Length - 1

                '... if any byte in the thumbprint from the claim does NOT match the corresponding
                ' byte from the thumbprint in the BookStoreSTS.com certificate, return false
                If issuerThumbprint(i) <> certThumbprint(i) Then
                    Return False
                End If

            Next

            ' If we get through all the above checks, return true (ClaimSet was issued by HomeRealmSTS.com)
            Return True

        End Function

        Private Shared Function ConvertedAnonymousMethod1(ByVal target As ClaimSet) As Boolean
            Dim certClaimSet As X509CertificateClaimSet = TryCast(target.Issuer, X509CertificateClaimSet)
            Return certClaimSet IsNot Nothing AndAlso certClaimSet.X509Certificate.Subject = "CN=HomeRealmSTS.com"
        End Function

#End Region

    End Class

End Namespace

