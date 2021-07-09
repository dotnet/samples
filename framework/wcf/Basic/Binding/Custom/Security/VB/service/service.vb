' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.ServiceModel

Namespace Microsoft.Samples.Security

    ' Define a duplex service contract.
    ' A duplex contract consists of two interfaces.
    ' The primary interface is used to send messages from client to service.
    ' The callback interface is used to send messages from service back to client.
    ' ICalculatorDuplex allows one to perform multiple operations on a running result.
    ' The result is sent back after each operation on the ICalculatorCallback interface.
    <ServiceContract([Namespace]:="http://Microsoft.Samples.Security", SessionMode:=SessionMode.Required, CallbackContract:=GetType(ICalculatorDuplexCallback))> _
    Public Interface ICalculatorDuplex

        <OperationContract(IsOneWay:=True)> _
        Sub Clear()
        <OperationContract(IsOneWay:=True)> _
        Sub AddTo(ByVal n As Double)
        <OperationContract(IsOneWay:=True)> _
        Sub SubtractFrom(ByVal n As Double)
        <OperationContract(IsOneWay:=True)> _
        Sub MultiplyBy(ByVal n As Double)
        <OperationContract(IsOneWay:=True)> _
        Sub DivideBy(ByVal n As Double)

    End Interface

    ' The callback interface is used to send messages from service back to client.
    ' The Result operation will return the current result after each operation.
    ' The Equation opertion will return the complete equation after Clear() is called.
    Public Interface ICalculatorDuplexCallback

        <OperationContract(IsOneWay:=True)> _
        Sub Result(ByVal result As Double)
        <OperationContract(IsOneWay:=True)> _
        Sub Equation(ByVal eqn As String)

    End Interface

    ' Service class which implements a duplex service contract.
    ' Use an InstanceContextMode of PrivateSession to store the result
    ' An instance of the service will be bound to each duplex session
    <ServiceBehavior(InstanceContextMode:=InstanceContextMode.PerSession)> _
    Public Class CalculatorService
        Implements ICalculatorDuplex

        Private result As Double
        Private equation As String
        Private callback As ICalculatorDuplexCallback = Nothing

        Public Sub New()

            result = 0
            equation = result.ToString()
            callback = OperationContext.Current.GetCallbackChannel(Of ICalculatorDuplexCallback)()

        End Sub

        Public Sub Clear() Implements ICalculatorDuplex.Clear

            callback.Equation(equation + " = " + result.ToString())
            result = 0
            equation = result.ToString()

        End Sub

        Public Sub AddTo(ByVal n As Double) Implements ICalculatorDuplex.AddTo

            result += n
            equation += " + " + n.ToString()
            callback.Result(result)

        End Sub

        Public Sub SubtractFrom(ByVal n As Double) Implements ICalculatorDuplex.SubtractFrom

            result -= n
            equation += " - " + n.ToString()
            callback.Result(result)

        End Sub

        Public Sub MultiplyBy(ByVal n As Double) Implements ICalculatorDuplex.MultiplyBy

            result *= n
            equation += " * " + n.ToString()
            callback.Result(result)

        End Sub

        Public Sub DivideBy(ByVal n As Double) Implements ICalculatorDuplex.DivideBy

            result /= n
            equation += " / " + n.ToString()
            callback.Result(result)

        End Sub

        Public Shared Sub Main()

            ' Create a ServiceHost for the CalculatorService type
            Using host As New ServiceHost(GetType(CalculatorService))

                ' Open the ServiceHostBase to create listeners and start listening for messages.
                host.Open()

                ' The service can now be accessed.
                Console.WriteLine("The service is ready.")
                Console.WriteLine("Press <ENTER> to terminate service.")
                Console.WriteLine()
                Console.ReadLine()

            End Using

        End Sub

    End Class

End Namespace

