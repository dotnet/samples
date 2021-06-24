'----------------------------------------------------------------
' Copyright (c) Microsoft Corporation.  All rights reserved. 
'----------------------------------------------------------------

Imports Microsoft.VisualBasic
Imports System
Imports System.ServiceModel.Web
Imports System.Xml

Namespace Microsoft.Samples.WeaklyTypedJson

    Friend Class Program

        Shared Sub Main()
            Dim baseAddress As New Uri("http://localhost:8000/")

            Using host As New WebServiceHost(GetType(ServerSideProfileService), baseAddress)
                ' Open the service host, service is now listening
                host.Open()

                Console.WriteLine("Service listening at {0}.", baseAddress)
                Console.WriteLine("To view its JSON output, point your web browser to {0}GetMemberProfile.", baseAddress)
                Console.WriteLine()


                Using cf As WebChannelFactory(Of IClientSideProfileService) = New WebChannelFactory(Of IClientSideProfileService)(baseAddress)
                    ' Create client side proxy
                    Dim channel As IClientSideProfileService = cf.CreateChannel()

                    ' Make a request to the service and get the Json response
                    Dim reader As XmlDictionaryReader = channel.GetMemberProfile().GetReaderAtBodyContents()

                    ' Go through the Json as though it's a dictionary. There is no need to map it to a CLR type.
                    Dim json As New JsonObject(reader)


                    Dim name As String = json("root")("personal")("name")
                    Dim age As Integer = json("root")("personal")("age")
                    Dim height As Double = json("root")("personal")("height")
                    Dim isSingle As Boolean = json("root")("personal")("isSingle")
                    Dim luckyNumbers() As Integer = {json("root")("personal")("luckyNumbers")(0), json("root")("personal")("luckyNumbers")(1), json("root")("personal")("luckyNumbers")(2)}
                    Dim favoriteBands() As String = {json("root")("favoriteBands")(0), json("root")("favoriteBands")(1)}

                    Console.WriteLine("This is {0}'s page. I am {1} years old and I am {2} meters tall.", name, age, height)
                    Console.WriteLine("I am {0}single.", If((isSingle), "", "not "))
                    Console.WriteLine("My lucky numbers are {0}, {1}, and {2}.", luckyNumbers(0), luckyNumbers(1), luckyNumbers(2))
                    Console.WriteLine("My favorite bands are {0} and {1}.", favoriteBands(0), favoriteBands(1))
                End Using

                Console.WriteLine()
                Console.WriteLine("Press Enter to terminate...")
                Console.ReadLine()
            End Using
        End Sub
    End Class
End Namespace
