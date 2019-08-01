Imports System.Collections

Public Class Example
   Public Class ReverserClass : Implements IComparer
      ' Call CaseInsensitiveComparer.Compare with the parameters reversed.
      Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer _
             Implements IComparer.Compare
         Return New CaseInsensitiveComparer().Compare(y, x)
      End Function 
   End Class

   Public Shared Sub Main()
      ' Initialize a string array.
      Dim words() As String = { "The", "quick", "brown", "fox", "jumps", "over",
                         "the", "lazy", "dog" }

      ' Display the array values.
      Console.WriteLine("The array initially contains the following values:")
      PrintIndexAndValues(words)

      ' Sort the array values of the ArrayList using the default comparer.
      Array.Sort(words)
      Console.WriteLine("After sorting with the default comparer:")
      PrintIndexAndValues(words)

      ' Sort the array values using the reverse case-insensitive comparer.
      Array.Sort(words, new ReverserClass())
      Console.WriteLine("After sorting with the reverse case-insensitive comparer:")
      PrintIndexAndValues(words)
   End Sub 

   Public Shared Sub PrintIndexAndValues(list As IEnumerable)
      Dim i As Integer = 0
      For Each item In  list
         Console.WriteLine($"   [{i}]:  {item}")
         i += 1
      Next
      Console.WriteLine()
   End Sub 
End Class
' The example displays the following output:
'       The array initially contains the following values:
'          [0]:  The
'          [1]:  quick
'          [2]:  brown
'          [3]:  fox
'          [4]:  jumps
'          [5]:  over
'          [6]:  the
'          [7]:  lazy
'          [8]:  dog
'       
'       After sorting with the default comparer:
'          [0]:  brown
'          [1]:  dog
'          [2]:  fox
'          [3]:  jumps
'          [4]:  lazy
'          [5]:  over
'          [6]:  quick
'          [7]:  the
'          [8]:  The
'       
'       After sorting with the reverse case-insensitive comparer:
'          [0]:  the
'          [1]:  The
'          [2]:  quick
'          [3]:  over
'          [4]:  lazy
'          [5]:  jumps
'          [6]:  fox
'          [7]:  dog
'          [8]:  brown

