'<snippetNamespaces>
Imports System
Imports System.Collections.Generic
Imports System.Collections
Imports System.Data.Common
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.EntityClient
Imports System.Data.Metadata.Edm
Imports System.IO
' Add AdventureWorksModel prepended with the root namespace for the project.
'Imports ProjectName.AdventureWorksModel
'</snippetNamespaces>
Imports System.Data.Objects
Imports System.Data.Objects.DataClasses
Imports eSQLExamplesVB.AdventureWorksModel
Imports System.Transactions

Module Program
    Sub BuildingConnectionStringWithEntityCommand()
        '<snippetBuildingConnectionStringWithEntityCommand>
        ' Specify the provider name, server and database.
        Dim providerName As String = "System.Data.SqlClient"
        Dim serverName As String = "."
        Dim databaseName As String = "AdventureWorks"

        ' Initialize the connection string builder for the
        ' underlying provider.
        Dim sqlBuilder As New SqlConnectionStringBuilder

        ' Set the properties for the data source.
        sqlBuilder.DataSource = serverName
        sqlBuilder.InitialCatalog = databaseName
        sqlBuilder.IntegratedSecurity = True

        ' Build the SqlConnection connection string.
        Dim providerString As String = sqlBuilder.ToString

        ' Initialize the EntityConnectionStringBuilder.
        Dim entityBuilder As New EntityConnectionStringBuilder

        'Set the provider name.
        entityBuilder.Provider = providerName
        ' Set the provider-specific connection string.
        entityBuilder.ProviderConnectionString = providerString
        ' Set the Metadata location to the current directory.
        entityBuilder.Metadata = "res://*/AdventureWorksModel.csdl|" & _
                                    "res://*/AdventureWorksModel.ssdl|" & _
                                    "res://*/AdventureWorksModel.msl"

        Console.WriteLine(entityBuilder.ToString)

        Using conn As EntityConnection = New EntityConnection(entityBuilder.ToString)
            conn.Open()
            Console.WriteLine("Just testing the connection.")
            conn.Close()
        End Using
        '</snippetBuildingConnectionStringWithEntityCommand>
    End Sub
    Sub Main()
        BuildingConnectionStringWithEntityCommand()
    End Sub

End Module
