Imports System.Globalization

Public Module Example
   Public Sub Main()
      Dim dateStrings() = { "2018-08-18T12:45:16.0000000Z",
                            "2018/08/18T12:45:16.0000000Z",
                            "2018-18-08T12:45:16.0000000Z",
                            "2018-08-18T12:45:16.0000000",                               
                            " 2018-08-18T12:45:16.0000000Z ",
                            "2018-08-18T12:45:16.0000000+02:00",
                            "2018-08-18T12:45:16.0000000-07:00" } 
      
      ParseWithISO8601(dateStrings, DateTimeStyles.None)
      Console.WriteLine($"{vbCrLf}-----{vbCrLf}")
      ParseWithISO8601(dateStrings, DateTimeStyles.AllowWhiteSpaces)
      Console.WriteLine($"{vbCrLf}-----{vbCrLf}")
      ParseWithISO8601(dateStrings, DateTimeStyles.AdjustToUniversal)
      Console.WriteLine($"{vbCrLf}-----{vbCrLf}")
      ParseWithISO8601(dateStrings, DateTimeStyles.AssumeLocal)
      Console.WriteLine($"{vbCrLf}-----{vbCrLf}")
      ParseWithISO8601(dateStrings, DateTimeStyles.AssumeUniversal)   
   End Sub

   Private Sub ParseWithISO8601(dateStrings() As String, styles As DateTimeStyles)
      Console.WriteLine($"Parsing with {styles}:")
      For Each dateStr In dateStrings
         Try 
            Dim dat = DateTimeOffset.ParseExact(dateString, "O", Nothing, styles)
            Console.WriteLine($"   {dateString} --> {dat:yyyy-MM-dd HH:mm:ss.FF zzz}")
         catch e As FormatException
            Console.WriteLine($"   FormatException: Unable to convert '{dateString}'")
         End Try   
      Next 
   End Sub
End Module
' The example displays the following output:
'      Parsing with None:
'         2018-08-18T12:45:16.0000000Z --> 2018-08-18 12:45:16 +00:00
'         FormatException: Unable to convert '2018/08/18T12:45:16.0000000Z'
'         FormatException: Unable to convert '2018-18-08T12:45:16.0000000Z'
'         2018-08-18T12:45:16.0000000 --> 2018-08-18 12:45:16 -07:00
'         FormatException: Unable to convert ' 2018-08-18T12:45:16.0000000Z '
'         2018-08-18T12:45:16.0000000+02:00 --> 2018-08-18 12:45:16 +02:00
'         2018-08-18T12:45:16.0000000-07:00 --> 2018-08-18 12:45:16 -07:00
'
'      -----
'
'      Parsing with AllowWhiteSpaces:
'         2018-08-18T12:45:16.0000000Z --> 2018-08-18 12:45:16 +00:00
'         FormatException: Unable to convert '2018/08/18T12:45:16.0000000Z'
'         FormatException: Unable to convert '2018-18-08T12:45:16.0000000Z'
'         2018-08-18T12:45:16.0000000 --> 2018-08-18 12:45:16 -07:00
'         2018-08-18T12:45:16.0000000Z  --> 2018-08-18 12:45:16 +00:00
'         2018-08-18T12:45:16.0000000+02:00 --> 2018-08-18 12:45:16 +02:00
'         2018-08-18T12:45:16.0000000-07:00 --> 2018-08-18 12:45:16 -07:00
'
'      -----
'
'      Parsing with AdjustToUniversal:
'         2018-08-18T12:45:16.0000000Z --> 2018-08-18 12:45:16 +00:00
'         FormatException: Unable to convert '2018/08/18T12:45:16.0000000Z'
'         FormatException: Unable to convert '2018-18-08T12:45:16.0000000Z'
'         2018-08-18T12:45:16.0000000 --> 2018-08-18 19:45:16 +00:00
'         FormatException: Unable to convert ' 2018-08-18T12:45:16.0000000Z '
'         2018-08-18T12:45:16.0000000+02:00 --> 2018-08-18 10:45:16 +00:00
'         2018-08-18T12:45:16.0000000-07:00 --> 2018-08-18 19:45:16 +00:00
'
'      -----
'
'      Parsing with AssumeLocal:
'         2018-08-18T12:45:16.0000000Z --> 2018-08-18 12:45:16 +00:00
'         FormatException: Unable to convert '2018/08/18T12:45:16.0000000Z'
'         FormatException: Unable to convert '2018-18-08T12:45:16.0000000Z'
'         2018-08-18T12:45:16.0000000 --> 2018-08-18 12:45:16 -07:00
'         FormatException: Unable to convert ' 2018-08-18T12:45:16.0000000Z '
'         2018-08-18T12:45:16.0000000+02:00 --> 2018-08-18 12:45:16 +02:00
'         2018-08-18T12:45:16.0000000-07:00 --> 2018-08-18 12:45:16 -07:00
'
'      -----
'
'      Parsing with AssumeUniversal:
'         2018-08-18T12:45:16.0000000Z --> 2018-08-18 12:45:16 +00:00
'         FormatException: Unable to convert '2018/08/18T12:45:16.0000000Z'
'         FormatException: Unable to convert '2018-18-08T12:45:16.0000000Z'
'         2018-08-18T12:45:16.0000000 --> 2018-08-18 12:45:16 +00:00
'         FormatException: Unable to convert ' 2018-08-18T12:45:16.0000000Z '
'         2018-08-18T12:45:16.0000000+02:00 --> 2018-08-18 12:45:16 +02:00
'         2018-08-18T12:45:16.0000000-07:00 --> 2018-08-18 12:45:16 -07:00
