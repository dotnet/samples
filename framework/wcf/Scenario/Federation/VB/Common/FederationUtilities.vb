' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System

Imports System.IdentityModel.Tokens

Imports System.Security.Cryptography.X509Certificates
Imports System.Security.Permissions

Namespace Microsoft.Samples.Federation

    Public Module FederationUtilities

        ''' <summary>
        ''' Utility method to get a certificate from a given store
        ''' </summary>
        ''' <param name="storeName">Name of certificate store (e.g. My, TrustedPeople)</param>
        ''' <param name="storeLocation">Location of certificate store (e.g. LocalMachine, CurrentUser)</param>
        ''' <param name="subjectDistinguishedName">The Subject Distinguished Name of the certificate</param>
        ''' <returns>The specified X509 certificate</returns>
        Private Function LookupCertificate(ByVal storeName As StoreName, ByVal storeLocation As StoreLocation, ByVal subjectDistinguishedName As String) As X509Certificate2

            Dim store As X509Store = Nothing
            Try

                store = New X509Store(storeName, storeLocation)
                store.Open(OpenFlags.[ReadOnly])
                Dim certs As X509Certificate2Collection = store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, subjectDistinguishedName, False)

                If certs.Count <> 1 Then

                    Throw New Exception([String].Format("FedUtil: Certificate {0} not found or more than one certificate found", subjectDistinguishedName))

                End If
                Return DirectCast(certs(0), X509Certificate2)

            Finally

                If store IsNot Nothing Then
                    store.Close()
                End If

            End Try

        End Function

#Region "GetX509TokenFromCert()"
        ''' <summary>
        ''' Utility method to get a X509 Token from a given certificate
        ''' </summary>
        ''' <param name="storeName">Name of certificate store (e.g. My, TrustedPeople)</param>
        ''' <param name="storeLocation">Location of certificate store (e.g. LocalMachine, CurrentUser)</param>
        ''' <param name="subjectDistinguishedName">The Subject Distinguished Name of the certificate</param>
        ''' <returns>The corresponding X509 Token</returns>
        Public Function GetX509TokenFromCert(ByVal storeName As StoreName, ByVal storeLocation As StoreLocation, ByVal subjectDistinguishedName As String) As X509SecurityToken

            Dim certificate As X509Certificate2 = LookupCertificate(storeName, storeLocation, subjectDistinguishedName)
            Dim t As New X509SecurityToken(certificate)
            Return t

        End Function
#End Region

#Region "GetCertificateThumbprint"
        ''' <summary>
        ''' Utility method to get an X509 Certificate thumbprint
        ''' </summary>
        ''' <param name="storeName">Name of certificate store (e.g. My, TrustedPeople)</param>
        ''' <param name="storeLocation">Location of certificate store (e.g. LocalMachine, CurrentUser)</param>
        ''' <param name="subjectDistinguishedName">The Subject Distinguished Name of the certificate</param>
        ''' <returns>The corresponding X509 Certificate thumbprint</returns>
        Public Function GetCertificateThumbprint(ByVal storeName As StoreName, ByVal storeLocation As StoreLocation, ByVal subjectDistinguishedName As String) As Byte()

            Dim certificate As X509Certificate2 = LookupCertificate(storeName, storeLocation, subjectDistinguishedName)
            Return certificate.GetCertHash()

        End Function
#End Region

    End Module

End Namespace
