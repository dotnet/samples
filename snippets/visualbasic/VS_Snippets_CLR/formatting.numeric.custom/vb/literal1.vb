Public Module Example
   Public Sub Main()
      Dim n As Integer = 9
      Console.WriteLine($"{n:##.0\%}") 
      Console.WriteLine($"{n:\'##\'}")
      Console.WriteLine($"{n:\\##\\}")    

      Console.WriteLine($"{n:##.0'%'}")  
      Console.WriteLine($"{n:'\'##'\'}")      
   End Sub
End Module 
' The example displays the following output:
'      9.0%
'      '9'
'      \9\
'
'      9.0%
'      \9\
