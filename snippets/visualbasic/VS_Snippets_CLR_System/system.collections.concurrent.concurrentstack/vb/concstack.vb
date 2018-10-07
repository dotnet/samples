'<snippet1>
Imports System.Collections.Concurrent
Imports System.Threading

Class Example
    ' Demonstrates:
    '   ConcurrentStack<T>.PushRange();
    '   ConcurrentStack<T>.TryPopRange();
    Shared Sub Main()
        Dim numParallelTasks As Integer = 4
        Dim numItems As Integer = 1000
        Dim stack = New ConcurrentStack(Of Integer)()

        ' Push a range of values onto the stack concurrently
        Task.WaitAll(Enumerable.Range(0, numParallelTasks).[Select](
                     Function(i) Task.Factory.StartNew(
                        Function(state)
                            Dim index As Integer = CInt(state)
                            Dim array As Integer() = New Integer(numItems - 1) {}

                            For j As Integer = 0 To numItems - 1
                                array(j) = index + j
                            Next

                            Console.WriteLine($"Pushing an array of ints from {array(0)} to {array(numItems - 1)}")
                            stack.PushRange(array)
                        End Function, i * numItems, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.[Default])).ToArray())


        Dim numTotalElements As Integer = 4 * numItems
        Dim resultBuffer As Integer() = New Integer(numTotalElements - 1) {}
        Task.WaitAll(Enumerable.Range(0, numParallelTasks).[Select](
                     Function(i) Task.Factory.StartNew(
                        Function(obj)
                            Dim index As Integer = CInt(obj)
                            Dim result As Integer = stack.TryPopRange(resultBuffer, index, numItems)
                            Console.WriteLine($"TryPopRange expected {numItems}, got {result}.")
                        End Function, i * numItems, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.[Default])).ToArray())

        For i As Integer = 0 To numParallelTasks - 1
            ' Create a sequence we expect to see from the stack taking the last number of the range we inserted
            Dim expected = Enumerable.Range(resultBuffer(i * numItems + numItems - 1), numItems)

            ' Take the range we inserted, reverse it, and compare to the expected sequence
            Dim areEqual = expected.SequenceEqual(resultBuffer.Skip(i * numItems).Take(numItems).Reverse())

            If areEqual Then
                Console.WriteLine($"Expected a range of {expected.First()} to {expected.Last()}. Got {resultBuffer(i * numItems + numItems - 1)} to {resultBuffer(i * numItems)}")
            Else
                Console.WriteLine($"Unexpected consecutive ranges.")
            End If
        Next
    End Sub
End Class
'</snippet1>

'<snippet2>
Imports System.Collections.Concurrent

Class Example
    Shared Sub Main()
        Dim items As Integer = 10000
        Dim stack As ConcurrentStack(Of Integer) = New ConcurrentStack(Of Integer)()

        ' Create an action to push items onto the stack
        Dim pusher As Action = Function()
                                   For i As Integer = 0 To items - 1
                                       stack.Push(i)
                                   Next
                               End Function

        ' Run the action once
        pusher()

        Dim result As Integer = Nothing

        If stack.TryPeek(result) Then
            Console.WriteLine($"TryPeek() saw {result} on top of the stack.")
        Else
            Console.WriteLine("Could not peek most recently added number.")
        End If

        ' Empty the stack
        stack.Clear()

        If stack.IsEmpty Then
            Console.WriteLine("Cleared the stack.")
        End If

        ' Create an action to push and pop items
        Dim pushAndPop As Action = Function()
                                       Console.WriteLine($"Task started on {Task.CurrentId}")
                                       Dim item As Integer

                                       For i As Integer = 0 To items - 1
                                           stack.Push(i)
                                       Next

                                       For i As Integer = 0 To items - 1
                                           stack.TryPop(item)
                                       Next

                                       Console.WriteLine($"Task ended on {Task.CurrentId}")
                                   End Function

        ' Spin up five concurrent tasks of the action
        Dim tasks = New Task(4) {}
        For i As Integer = 0 To tasks.Length - 1
            tasks(i) = Task.Factory.StartNew(pushAndPop)
        Next

        ' Wait for all the tasks to finish up
        Task.WaitAll(tasks)

        If Not stack.IsEmpty Then
            Console.WriteLine("Did not take all the items off the stack")
        End If
    End Sub
End Class
'</snippet2>