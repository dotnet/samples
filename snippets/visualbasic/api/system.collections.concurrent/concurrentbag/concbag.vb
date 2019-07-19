'<snippet1>
Imports System.Collections.Concurrent

Module ConcurrentBagDemo
    ' Demonstrates:
    ' ConcurrentBag<T>.Add()
    ' ConcurrentBag<T>.IsEmpty
    ' ConcurrentBag<T>.TryTake()
    ' ConcurrentBag<T>.TryPeek()
    Sub Main()
        ' Add to ConcurrentBag concurrently
        Dim cb As New ConcurrentBag(Of Integer)()
        Dim bagAddTasks As New List(Of Task)()
        For i = 1 To 500
            Dim numberToAdd As Integer = i
            bagAddTasks.Add(Task.Run(Sub() cb.Add(numberToAdd)))
        Next

        ' Wait for all tasks to complete
        Task.WaitAll(bagAddTasks.ToArray())

        ' Consume the items in the bag
        Dim bagConsumeTasks As New List(Of Task)()
        Dim itemsInBag As Integer = 0
        While Not cb.IsEmpty
            bagConsumeTasks.Add(Task.Run(Sub()
                                             Dim item As Integer
                                             If cb.TryTake(item) Then
                                                 Console.WriteLine(item)
                                                 itemsInBag = itemsInBag + 1
                                             End If
                                         End Sub))
        End While

        Task.WaitAll(bagConsumeTasks.ToArray())

        Console.WriteLine($"There were {itemsInBag} items in the bag")

        ' Checks the bag for an item
        ' The bag should be empty and this should not print anything
        Dim unexpectedItem As Integer
        If cb.TryPeek(unexpectedItem) Then
            Console.WriteLine("Found an item in the bag when it should be empty")
        End If
    End Sub
End Module
'</snippet1>