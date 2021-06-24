' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System

Namespace Microsoft.Samples.Federation

    Public Class Constants

        Public Const BookNameHeaderNamespace As String = "http://tempuri.org/"
        Public Const BookNameHeaderName As String = "BookName"
        Public Const PurchaseClaimNamespace As String = "http://tempuri.org/"
        Public Const PurchaseAuthorizedClaim As String = PurchaseClaimNamespace & "PurchaseAuthorizedClaim"
        Public Const PurchaseLimitClaim As String = PurchaseClaimNamespace & "PurchaseLimitClaim"
        Public Const SamlTokenTypeUri As String = "http://docs.oasis-open.org/wss/oasis-wss-saml-token-profile-1.1#SAMLV1.1"

        Public Const BookStoreServiceAudienceUri As String = "http://localhost/FederationSample/BookStoreService/store.svc/buy"
        Public Const BookStoreSTSAudienceUri As String = "http://localhost/FederationSample/BookStoreSTS/STS.svc"

        ' Various constants for WS-Trust
        Public Class Trust

            Public Const NamespaceUri As String = "http://schemas.xmlsoap.org/ws/2005/02/trust"

            Public Class Actions

                Public Const Issue As String = "http://schemas.xmlsoap.org/ws/2005/02/trust/RST/Issue"
                Public Const IssueReply As String = "http://schemas.xmlsoap.org/ws/2005/02/trust/RSTR/Issue"

            End Class

            Public Class Attributes

                Public Const Context As String = "Context"

            End Class

            Public Class Elements

                Public Const KeySize As String = "KeySize"
                Public Const Entropy As String = "Entropy"
                Public Const BinarySecret As String = "BinarySecret"
                Public Const RequestSecurityToken As String = "RequestSecurityToken"
                Public Const RequestSecurityTokenResponse As String = "RequestSecurityTokenResponse"
                Public Const TokenType As String = "TokenType"
                Public Const RequestedSecurityToken As String = "RequestedSecurityToken"
                Public Const RequestedAttachedReference As String = "RequestedAttachedReference"
                Public Const RequestedUnattachedReference As String = "RequestedUnattachedReference"
                Public Const RequestedProofToken As String = "RequestedProofToken"
                Public Const ComputedKey As String = "ComputedKey"

            End Class

            Public Class ComputedKeyAlgorithms

                Public Const PSHA1 As String = "http://schemas.xmlsoap.org/ws/2005/02/trust/CK/PSHA1"

            End Class

        End Class

        ' Various constants for WS-Policy
        Public Class Policy

            Public Const NamespaceUri As String = "http://schemas.xmlsoap.org/ws/2004/09/policy"

            Public Class Elements

                Public Const AppliesTo As String = "AppliesTo"

            End Class

        End Class

        ' Various constants for WS-Addressing
        Public Class Addressing

            Public Const NamespaceUri As String = "http://www.w3.org/2005/08/addressing"

            Public Class Elements

                Public Const EndpointReference As String = "EndpointReference"

            End Class

        End Class

    End Class

End Namespace

