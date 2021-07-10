' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System

Imports System.Configuration

Imports System.Security.Cryptography.X509Certificates

Namespace Microsoft.Samples.Federation

    Class ServiceConstants

        ' Issuer name placed into issued tokens
        Friend Const SecurityTokenServiceName As String = "BookStore STS"

        ' Statics for location of certs
        Friend Shared ReadOnly CertStoreName As StoreName = StoreName.TrustedPeople
        Friend Shared ReadOnly CertStoreLocation As StoreLocation = StoreLocation.LocalMachine

        ' Statics initialized from app.config
        Friend Shared CertDistinguishedName As String
        Friend Shared TargetDistinguishedName As String
        Friend Shared IssuerDistinguishedName As String
        Friend Shared BookDB As String

#Region "Helper functions to load app settings from config"
        ''' <summary>
        ''' Helper function to load Application Settings from config
        ''' </summary>
        Public Shared Sub LoadAppSettings()

            BookDB = ConfigurationManager.AppSettings("bookDB")
            CheckIfLoaded(BookDB)
            BookDB = [String].Format("{0}\{1}", System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, BookDB)

            CertDistinguishedName = ConfigurationManager.AppSettings("certDistinguishedName")
            CheckIfLoaded(CertDistinguishedName)

            TargetDistinguishedName = ConfigurationManager.AppSettings("targetDistinguishedName")
            CheckIfLoaded(TargetDistinguishedName)
            IssuerDistinguishedName = ConfigurationManager.AppSettings("issuerDistinguishedName")
            CheckIfLoaded(IssuerDistinguishedName)

        End Sub

        ''' <summary>
        ''' Helper function to check if a required Application Setting has been specified in config.
        ''' Throw if some Application Setting has not been specified.
        ''' </summary>
        Private Shared Sub CheckIfLoaded(ByVal s As String)

            If [String].IsNullOrEmpty(s) Then

                Throw New ConfigurationErrorsException("Required Configuration Element(s) missing at BookStoreSTS. Please check the STS configuration file.")

            End If

        End Sub

#End Region

        Private Sub New()
        End Sub

    End Class

End Namespace

