' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.ServiceModel
Imports System.Threading
Imports System.Diagnostics

Namespace Microsoft.Samples.ServiceModel

    'The service contract is defined in generatedClient.vb, generated from the service by the svcutil tool.

    'Client implementation code.
    Class Client

        'the service exits when the flag becomes true
        Shared flag As Boolean

        Public Shared Sub Main()
            CreateCounter()
            Dim workerThread As New Thread(New ThreadStart(AddressOf DoWork))

            workerThread.Start()

            Console.WriteLine()
            Console.WriteLine("Press <ENTER> to terminate client once the output starts displaying.")
            Console.ReadLine()
            flag = True

            workerThread.Join()

        End Sub

        Public Shared Sub CreateCounter()

            Dim col As New CounterCreationDataCollection()

            ' Create two custom counter objects.
            Dim addCounter As New CounterCreationData()
            addCounter.CounterName = "AddCounter"
            addCounter.CounterHelp = "Custom Add counter "
            addCounter.CounterType = PerformanceCounterType.NumberOfItemsHEX32

            ' Add custom counter objects to CounterCreationDataCollection.
            col.Add(addCounter)

            ' Bind the counters to a PerformanceCounterCategory
            ' Check if the category already exists or not.
            If Not PerformanceCounterCategory.Exists("MyCategory") Then

                Dim category As PerformanceCounterCategory = PerformanceCounterCategory.Create("MyCategory", "My Perf Category Description ", PerformanceCounterCategoryType.Unknown, col)

            Else

                Console.WriteLine("Counter already exists")

            End If

        End Sub

        Private Shared Sub IncrementCounter()

            ' get an instance of our perf counter
            Dim counter As New PerformanceCounter()
            counter.CategoryName = "MyCategory"
            counter.CounterName = "AddCounter"
            counter.[ReadOnly] = False

            ' increment and close the perf counter
            counter.Increment()

            counter.Close()

        End Sub

        Private Shared Sub DoWork()

            ' Create a client with given client endpoint configuration
            Dim client As New CalculatorClient()

            Dim rand As New Random()

            While Not flag

                ' Call the Add service operation.
                Dim value1 As Double = CDbl(rand.Next(0, 5))
                Dim value2 As Double = CDbl(rand.Next(0, 5))
                Dim result As Double

                Select Case rand.[Next](0, 4)

                    Case (0)
                        IncrementCounter()
                        result = client.Add(value1, value2)
                        Console.WriteLine("Add({0},{1}) = {2}", value1, value2, result)
                        Exit Select
                    Case (1)
                        result = client.Subtract(value1, value2)
                        Console.WriteLine("Subtract({0},{1}) = {2}", value1, value2, result)
                        Exit Select
                    Case (2)
                        result = client.Multiply(value1, value2)
                        Console.WriteLine("Multiply({0},{1}) = {2}", value1, value2, result)
                        Exit Select
                    Case (3)
                        result = client.Divide(value1, value2)
                        Console.WriteLine("Divide({0},{1}) = {2}", value1, value2, result)
                        Exit Select

                End Select

                Thread.Sleep(500)

            End While

            'Closing the client gracefully closes the connection and cleans up resources
            client.Close()

        End Sub

    End Class

End Namespace
