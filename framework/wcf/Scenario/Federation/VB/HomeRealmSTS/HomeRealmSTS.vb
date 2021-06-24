' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System

Imports System.Collections.ObjectModel

Imports System.IdentityModel.Claims
Imports System.IdentityModel.Tokens

Imports System.ServiceModel

Imports System.Security.Permissions

Namespace Microsoft.Samples.Federation

    <ServiceBehavior(IncludeExceptionDetailInFaults:=True)> _
    Public Class HomeRealmSTS
        Inherits SecurityTokenService

        Public Sub New()

            MyBase.New(ServiceConstants.StsName, FederationUtilities.GetX509TokenFromCert(ServiceConstants.CertStoreName, ServiceConstants.CertStoreLocation, ServiceConstants.CertDistinguishedName), FederationUtilities.GetX509TokenFromCert(ServiceConstants.CertStoreName, ServiceConstants.CertStoreLocation, ServiceConstants.TargetDistinguishedName))

        End Sub

        Private Shared Function GetPurchaseLimit() As Double

            ' give every authenticated caller the configured purchase limit
            Return ServiceConstants.PurchaseLimit

        End Function

        ''' <summary>
        ''' Overrides the GetIssuedClaims from the SecurityTokenService Base Class
        ''' to return a valid user claim and purchase limit claim with the appropriate purchase limit 
        ''' for the user
        ''' </summary>
        Protected Overloads Overrides Function GetIssuedClaims(ByVal requestSecurityToken As RequestSecurityToken) As Collection(Of SamlAttribute)

            Dim caller As String = ServiceSecurityContext.Current.PrimaryIdentity.Name
            Dim purchaseLimit As Double = GetPurchaseLimit()

            ' Create Name and PurchaseLimit claims
            Dim samlAttributes As New Collection(Of SamlAttribute)()
            samlAttributes.Add(New SamlAttribute(Claim.CreateNameClaim(caller)))
            samlAttributes.Add(New SamlAttribute(New Claim(Constants.PurchaseLimitClaim, purchaseLimit.ToString(), Rights.PossessProperty)))
            Return samlAttributes

        End Function

        ''' <summary>
        ''' This method adds the audience uri restriction condition to the SAML assetion.
        ''' </summary>
        ''' <param name="samlConditions">The saml condition collection where the audience uri restriction condition will be added.</param>
        Public Overloads Overrides Sub AddAudienceRestrictionCondition(ByVal samlConditions As SamlConditions)
            samlConditions.Conditions.Add(New SamlAudienceRestrictionCondition(New Uri() {New Uri(Constants.BookStoreSTSAudienceUri)}))
        End Sub

    End Class

End Namespace

