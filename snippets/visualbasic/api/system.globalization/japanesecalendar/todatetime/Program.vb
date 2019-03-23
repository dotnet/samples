Imports System.Globalization

Module Program
    Dim cal As Calendar
    Dim jaJp As CultureInfo
    
    Public Sub Main()
        cal = New JapaneseCalendar()
        jaJp = New CultureInfo("ja-JP")
        jaJp.DateTimeFormat.Calendar = cal
        Dim date1 = cal.ToDateTime(2,1,1,0,0,0,0,JapaneseCalendar.CurrentEra)
        Console.WriteLine($"Japanese calendar date: {date1.ToString("D", jaJp)}, " +
                          $"Gregorian calendar date: {date1.ToString("D", CultureInfo.InvariantCulture)}")

        Dim date2 = cal.ToDateTime(6,11,7,0,0,0,0,GetEraIndex("大正"))
        Console.WriteLine($"Japanese calendar date: {date2.ToString("D", jaJp)}, " +
                          $"Gregorian calendar date: {date2.ToString("D", CultureInfo.InvariantCulture)}")
    End Sub

    Private Function GetEraIndex(eraName As String) As Integer
        For Each ctr in cal.Eras
            If jaJp.DateTimeFormat.GetEraName(ctr) = eraName Then Return ctr
        Next

        Return 0 
    End Function
End Module
' The example displays the following output:
'   Japanese calendar date: 平成2年1月1日, Gregorian calendar date: Monday, 01 January 1990
'   Japanese calendar date: 大正6年11月7日, Gregorian calendar date: Wednesday, 07 November 1917
