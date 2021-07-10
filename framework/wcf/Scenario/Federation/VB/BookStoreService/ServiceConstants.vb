' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System

Imports System.Configuration

Imports System.Security.Cryptography.X509Certificates

Namespace Microsoft.Samples.Federation

    Class ServiceConstants

#Region "BookStore Service-Wide Constants"
        ' The following two Action strings are the defaults created by the OperationContract attribute
        Friend Const BrowseBooksAction As String = "http://tempuri.org/IBrowseBooks/BrowseBooks"
        Friend Const BuyBookAction As String = "http://tempuri.org/IBuyBook/BuyBook"

        ' Statics for location of certs
        Friend Shared CertStoreName As StoreName = StoreName.TrustedPeople
        Friend Shared CertStoreLocation As StoreLocation = StoreLocation.LocalMachine

        ' Statics initialized from app.config
        Friend Shared BookDB As String
        Friend Shared IssuerCertDistinguishedName As String
#End Region

#Region "Helper functions to load app settings from config"
        ''' <summary>
        ''' Helper function to load Application Settings from config
        ''' </summary>
        Public Shared Sub LoadAppSettings()

            BookDB = ConfigurationManager.AppSettings("bookDB")
            CheckIfLoaded(BookDB)
            BookDB = [String].Format("{0}\{1}", System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, BookDB)

            IssuerCertDistinguishedName = ConfigurationManager.AppSettings("issuerCertDistinguishedName")
            CheckIfLoaded(IssuerCertDistinguishedName)

        End Sub

        ''' <summary>
        ''' Helper function to check if a required Application Setting has been specified in config.
        ''' Throw if some Application Setting has not been specified.
        ''' </summary>
        Private Shared Sub CheckIfLoaded(ByVal s As String)

            If [String].IsNullOrEmpty(s) Then

                Throw New ConfigurationErrorsException("Required Configuration Element(s) missing at BookStoreService. Please check the service configuration file.")

            End If

        End Sub
#End Region

        Private Sub New()
        End Sub

    End Class

End Namespace

