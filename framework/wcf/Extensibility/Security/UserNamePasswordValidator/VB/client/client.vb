'-----------------------------------------------------------------------------
' Copyright (c) Microsoft Corporation.  All rights reserved.
'-----------------------------------------------------------------------------

Namespace Microsoft.Samples.UserNamePasswordValidator
    'The service contract is defined in generatedProxy.vb, generated from the service by the svcutil tool.
	'Client implementation code.
	Friend Class Client
		Shared Sub Main()
			' Get the username and password
			Console.WriteLine("Username authentication required.")
			Console.WriteLine("Provide a username.")
			Console.WriteLine("   Enter username: (test1)")
            Dim username = Console.ReadLine()
			Console.WriteLine("   Enter password:")
            Dim password = ""
			Dim info As ConsoleKeyInfo = Console.ReadKey(True)
			Do While info.Key <> ConsoleKey.Enter
				If info.Key <> ConsoleKey.Backspace Then
					If info.KeyChar <> ControlChars.NullChar Then
						password &= info.KeyChar
					End If
					info = Console.ReadKey(True)
				ElseIf info.Key = ConsoleKey.Backspace Then
					If password <> "" Then
						password = password.Substring(0, password.Length - 1)

					End If
					info = Console.ReadKey(True)
				End If
			Loop

            For i = 0 To password.Length - 1
                Console.Write("*")
            Next i
			Console.WriteLine()

            ' Create a proxy with Username endpoint configuration.
			Dim proxy As New CalculatorProxy("Username")

			Try
				proxy.ClientCredentials.UserName.UserName = username
				proxy.ClientCredentials.UserName.Password = password

				' Call the Add service operation.
                Dim value1 = 100.0R
                Dim value2 = 15.99R
                Dim result = proxy.Add(value1, value2)
				Console.WriteLine("Add({0},{1}) = {2}", value1, value2, result)

				' Call the Subtract service operation.
				value1 = 145.00R
				value2 = 76.54R
				result = proxy.Subtract(value1, value2)
				Console.WriteLine("Subtract({0},{1}) = {2}", value1, value2, result)

				' Call the Multiply service operation.
				value1 = 9.00R
				value2 = 81.25R
				result = proxy.Multiply(value1, value2)
				Console.WriteLine("Multiply({0},{1}) = {2}", value1, value2, result)

				' Call the Divide service operation.
				value1 = 22.00R
				value2 = 7.00R
				result = proxy.Divide(value1, value2)
				Console.WriteLine("Divide({0},{1}) = {2}", value1, value2, result)
				proxy.Close()
			Catch e As TimeoutException
				Console.WriteLine("Call timed out : {0}", e.Message)
				proxy.Abort()
			Catch e As Exception
				Console.WriteLine("Call failed:")
				Do While e IsNot Nothing
					Console.WriteLine(vbTab & "{0}", e.Message)
					e = e.InnerException
				Loop
				proxy.Abort()
			End Try

			Console.WriteLine()
			Console.WriteLine("Press <ENTER> to terminate client.")
			Console.ReadLine()
		End Sub
	End Class
End Namespace

