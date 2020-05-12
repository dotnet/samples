Imports System.Console
Imports System.ComponentModel.Composition
Imports System.ComponentModel.Composition.Hosting
Imports MefCalculator.MefCalculatorInterfaces

Public Module Program

    ReadOnly MyApplicationPath As String = Reflection.Assembly.GetEntryAssembly.Location
    ReadOnly MyApplicationDirectory As String = IO.Path.GetDirectoryName(MyApplicationPath)

    <Export(GetType(ICalculator))>
    Public Class MySimpleCalculator
        Implements ICalculator

        <ImportMany()>
        Public Property Operations As IEnumerable(Of Lazy(Of IOperation, IOperationData))

        Public Function Calculate(input As String) As String Implements ICalculator.Calculate
            Dim left, right As Integer
            Dim operation As Char
            Dim fn = FindFirstNonDigit(input) 'Finds the operator
            If fn < 0 Then
                Return "Could not parse command."
            End If
            operation = input(fn)
            Try
                left = Integer.Parse(input.Substring(0, fn))
                right = Integer.Parse(input.Substring(fn + 1))
            Catch ex As Exception
                Return "Could not parse command."
            End Try
            For Each i As Lazy(Of IOperation, IOperationData) In Operations
                If i.Metadata.Symbol = operation Then
                    Return i.Value.Operate(left, right).ToString()
                End If
            Next
            Return "Operation not found!"
        End Function

        Private Function FindFirstNonDigit(s As String) As Integer
            For i = 0 To If(s?.Length, 0) - 1
                If (Not (Char.IsDigit(s(i)))) Then Return i
            Next
            Return -1
        End Function

    End Class

    Private Class MySimpleCalculatorComposition

        ReadOnly _container As CompositionContainer
        Public ReadOnly HasExtensions As Boolean

        <Import(GetType(ICalculator))>
        Public Property Calculator As ICalculator

        Public Sub New()

            ' An aggregate catalog that combines multiple catalogs
            Dim catalog = New AggregateCatalog()

            ' Adds all the parts found in the MefCalculator assembly (class MefCalculatorInterfaces)
            catalog.Catalogs.Add(New AssemblyCatalog(GetType(MefCalculator.MefCalculatorInterfaces).Assembly))

            ' Adds all the parts found in the same assembly as this class (MySimpleCalculatorComposer)
            catalog.Catalogs.Add(New AssemblyCatalog(GetType(MySimpleCalculatorComposition).Assembly))

            ' Add parts which can be found in the Extensions subdirectory.
            Dim extensionsDir = IO.Path.Combine(MyApplicationDirectory, "Extensions")
            If IO.Directory.Exists(extensionsDir) Then
                catalog.Catalogs.Add(New DirectoryCatalog(extensionsDir))
                HasExtensions = True
            End If

            ' Create the CompositionContainer with the parts in the catalog
            _container = New CompositionContainer(catalog)

            ' Fill the imports of this object
            Try
                _container.ComposeParts(Me)
            Catch ex As Exception
                WriteLine(ex.ToString)
            End Try

        End Sub

    End Class

    Sub Main(args As String())
        Dim o As New MySimpleCalculatorComposition()
        If o.HasExtensions Then WriteLine("Extensions loaded.")
        WriteLine("Enter Command:")
        Do
            Dim s = ReadLine()
            If s Is Nothing Then Exit Do ' Detect Ctrl+C
            WriteLine(o.Calculator.Calculate(s))
        Loop
        WriteLine("Exited")
    End Sub

End Module
