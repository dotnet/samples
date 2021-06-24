' Copyright (c) Microsoft Corporation. All rights reserved.

Imports System

Namespace Microsoft.Samples.WS2007FederationHttpBinding
    Public Class Constants
        Public Class Addressing
            Public Const NamespaceUri As String = "http://www.w3.org/2005/08/addressing"
            Public Const NamespaceUriAugust2004 As String = "http://schemas.xmlsoap.org/ws/2004/08/addressing"

            Public Class Elements
                Public Const EndpointReference As String = "EndpointReference"
            End Class
        End Class

        Public Class Policy
            Public Const NamespaceUri As String = "http://schemas.xmlsoap.org/ws/2004/09/policy"

            Public Class Elements
                Public Const AppliesTo As String = "AppliesTo"
            End Class
        End Class

        ' Various constants for WS-Trust13
        Public Class Trust13
            Public Const NamespaceUri As String = "http://docs.oasis-open.org/ws-sx/ws-trust/200512"

            Public Class Actions
                Public Const Issue As String = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/RST/Issue"
                Public Const IssueReply As String = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/RSTRC/IssueFinal"
            End Class

            Public Class Attributes
                Public Const Context As String = "Context"
                Public Const Type As String = "Type"
            End Class

            Public Class Elements
                Public Const KeySize As String = "KeySize"
                Public Const KeyType As String = "KeyType"
                Public Const UseKey As String = "UseKey"
                Public Const Entropy As String = "Entropy"
                Public Const BinarySecret As String = "BinarySecret"
                Public Const RequestSecurityToken As String = "RequestSecurityToken"
                Public Const RequestSecurityTokenResponseCollection As String = "RequestSecurityTokenResponseCollection"
                Public Const RequestSecurityTokenResponse As String = "RequestSecurityTokenResponse"
                Public Const RequestType As String = "RequestType"
                Public Const TokenType As String = "TokenType"
                Public Const RequestedSecurityToken As String = "RequestedSecurityToken"
                Public Const RequestedAttachedReference As String = "RequestedAttachedReference"
                Public Const RequestedUnattachedReference As String = "RequestedUnattachedReference"
                Public Const RequestedProofToken As String = "RequestedProofToken"
                Public Const ComputedKey As String = "ComputedKey"
                Public Const Claims As String = "Claims"
            End Class

            Public Class RequestTypes
                Public Const Issue As String = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/Issue"
            End Class

            Public Class KeyTypes
                Public Const [Public] As String = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/PublicKey"
                Public Const Symmetric As String = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/SymmetricKey"
            End Class

            Public Class BinarySecretTypes
                Public Const AsymmetricKey As String = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/AsymmetricKey"
                Public Const SymmetricKey As String = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/SymmetricKey"
                Public Const Nonce As String = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/Nonce"
            End Class

            Public Class ComputedKeyAlgorithms
                Public Const PSHA1 As String = "http://docs.oasis-open.org/ws-sx/ws-trust/200512/CK/PSHA1"
            End Class
        End Class
    End Class
End Namespace
