'  Copyright (c) Microsoft Corporation. All rights reserved.

Imports System.Threading

Namespace Microsoft.ServiceModel.Samples
	''' <summary>
	''' A generic base class for IAsyncResult implementations
	''' that wraps a ManualResetEvent.
	''' </summary>
	Friend MustInherit Class AsyncResult
		Implements IAsyncResult
		Private callback As AsyncCallback
		Private state As Object

        Private completedSynchronously_local As Boolean
        Private endCalled As Boolean
        Private exception As Exception

        Private isCompleted_local As Boolean
        Private manualResetEvent As ManualResetEvent

        Private thisLock_local As Object

        Protected Sub New(ByVal callback As AsyncCallback, ByVal state As Object)
            Me.callback = callback
            Me.state = state
            Me.thisLock_local = New Object()
        End Sub

        Public ReadOnly Property AsyncState() As Object Implements IAsyncResult.AsyncState
            Get
                Return state
            End Get
        End Property

        Public ReadOnly Property AsyncWaitHandle() As WaitHandle Implements IAsyncResult.AsyncWaitHandle
            Get
                If manualResetEvent IsNot Nothing Then
                    Return manualResetEvent
                End If

                SyncLock ThisLock
                    If manualResetEvent Is Nothing Then
                        manualResetEvent = New ManualResetEvent(isCompleted_local)
                    End If
                End SyncLock

                Return manualResetEvent
            End Get
        End Property

        Public ReadOnly Property CompletedSynchronously() As Boolean Implements IAsyncResult.CompletedSynchronously
            Get
                Return completedSynchronously_local
            End Get
        End Property

        Public ReadOnly Property IsCompleted() As Boolean Implements IAsyncResult.IsCompleted
            Get
                Return isCompleted_local
            End Get
        End Property

        Private ReadOnly Property ThisLock() As Object
            Get
                Return Me.thisLock_local
            End Get
        End Property

        ' Call this version of complete when your asynchronous operation is complete.  This will update the state
        ' of the operation and notify the callback.
        Protected Sub Complete(ByVal completedSynchronously As Boolean)
            If isCompleted_local Then
                ' It's a bug to call Complete twice.
                Throw New InvalidOperationException("Cannot call Complete twice")
            End If

            Me.completedSynchronously_local = completedSynchronously

            If completedSynchronously Then
                ' If we completedSynchronously, then there's no chance that the manualResetEvent was created so
                ' we don't need to worry about a race.
                Me.isCompleted_local = True
            Else
                SyncLock ThisLock
                    Me.isCompleted_local = True
                    If Me.manualResetEvent IsNot Nothing Then
                        Me.manualResetEvent.Set()
                    End If
                End SyncLock
            End If

            ' If the callback throws, there is a bug in the callback implementation.
            If callback IsNot Nothing Then
                callback(Me)
            End If
        End Sub

        ' Call this version of complete if you raise an exception during processing.  In addition to notifying
        ' the callback, it will capture the exception and store it to be thrown during AsyncResult.End.
        Protected Sub Complete(ByVal completedSynchronously As Boolean, ByVal exception As Exception)
            Me.exception = exception
            Complete(completedSynchronously)
        End Sub

        ' End should be called when the End function for the asynchronous operation is complete.  It
        ' ensures the asynchronous operation is complete, and does some common validation.
        Protected Shared Function [End](Of TAsyncResult As AsyncResult)(ByVal result As IAsyncResult) As TAsyncResult
            If result Is Nothing Then
                Throw New ArgumentNullException("result")
            End If

            Dim asyncResult As TAsyncResult = TryCast(result, TAsyncResult)

            If asyncResult Is Nothing Then
                Throw New ArgumentException("Invalid async result.", "result")
            End If

            If asyncResult.endCalled Then
                Throw New InvalidOperationException("Async object already ended.")
            End If

            asyncResult.endCalled = True

            If Not asyncResult.IsCompleted Then
                asyncResult.AsyncWaitHandle.WaitOne()
            End If

            If asyncResult.manualResetEvent IsNot Nothing Then
                asyncResult.manualResetEvent.Close()
            End If

            If asyncResult.exception IsNot Nothing Then
                Throw asyncResult.exception
            End If

            Return asyncResult
        End Function
    End Class

    'An AsyncResult that completes as soon as it is instantiated.
    Friend Class CompletedAsyncResult
        Inherits AsyncResult
        Public Sub New(ByVal callback As AsyncCallback, ByVal state As Object)
            MyBase.New(callback, state)
            Complete(True)
        End Sub

        Public Overloads Shared Sub [End](ByVal result As IAsyncResult)
            AsyncResult.End(Of CompletedAsyncResult)(result)
        End Sub
    End Class

    'A strongly typed AsyncResult.
    Friend MustInherit Class TypedAsyncResult(Of T)
        Inherits AsyncResult

        Private data_local As T

        Protected Sub New(ByVal callback As AsyncCallback, ByVal state As Object)
            MyBase.New(callback, state)
        End Sub

        Public ReadOnly Property Data() As T
            Get
                Return data_local
            End Get
        End Property

        Protected Overloads Sub Complete(ByVal data As T, ByVal completedSynchronously As Boolean)
            Me.data_local = data
            Complete(completedSynchronously)
        End Sub

        Public Overloads Shared Function [End](ByVal result As IAsyncResult) As T
            Dim typedResult As TypedAsyncResult(Of T) = AsyncResult.End(Of TypedAsyncResult(Of T))(result)
            Return typedResult.Data
        End Function
    End Class

	'A strongly typed AsyncResult that completes as soon as it is instantiated.
	Friend Class TypedCompletedAsyncResult(Of T)
		Inherits TypedAsyncResult(Of T)
		Public Sub New(ByVal data As T, ByVal callback As AsyncCallback, ByVal state As Object)
			MyBase.New(callback, state)
			Complete(data, True)
		End Sub

		Public Shadows Shared Function [End](ByVal result As IAsyncResult) As T
			Dim completedResult As TypedCompletedAsyncResult(Of T) = TryCast(result, TypedCompletedAsyncResult(Of T))
			If completedResult Is Nothing Then
				Throw New ArgumentException("Invalid async result.", "result")
			End If

			Return TypedAsyncResult(Of T).End(completedResult)
		End Function
	End Class
End Namespace
