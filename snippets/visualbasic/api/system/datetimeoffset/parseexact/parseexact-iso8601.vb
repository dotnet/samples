Public Module Example
   Public Sub Main()
      Dim dateStrings() As String = { "2018-08-18T12:45:16.0000000Z",
                                      "2018/08/18T12:45:16.0000000Z",
                                      "2018-18-08T12:45:16.0000000Z",
                                      " 2018-08-18T12:45:16.0000000Z ",
                                      "2018-08-18T12:45:16.0000000+02:00",
                                      "2018-08-18T12:45:16.0000000-07:00" } 
      
      For Each dateStr In dateStrings
         Try 
            Dim dat = DateTimeOffset.ParseExact(dateStr, "O", Nothing)
            Console.WriteLine($"{dateStr,-35} --> {dat:yyyy-MM-dd HH:mm:ss.FF zzz}")
         Catch  e As FormatException
            Console.WriteLine($"FormatException: Unable to convert '{dateStr}'")
         End Try   
      Next 
   End Sub
End Module
' The example displays the following output:
'      2018-08-18T12:45:16.0000000Z        --> 2018-08-18 12:45:16 +00:00
'      FormatException: Unable to convert '2018/08/18T12:45:16.0000000Z'
'      FormatException: Unable to convert '2018-18-08T12:45:16.0000000Z'
'      FormatException: Unable to convert ' 2018-08-18T12:45:16.0000000Z '
'      2018-08-18T12:45:16.0000000+02:00   --> 2018-08-18 12:45:16 +02:00
'      2018-08-18T12:45:16.0000000-07:00   --> 2018-08-18 12:45:16 -07:00
