'  Copyright (c) Microsoft Corporation. All rights reserved.

Imports System.ServiceModel
Imports System.ServiceModel.Diagnostics
Imports System.Threading

Namespace Microsoft.ServiceModel.Samples
	' ItemDequeuedCallback is called as an item is dequeued from the InputQueue.  The 
	' InputQueue lock is not held during the callback.  However, the user code is
	' not notified of the item being available until the callback returns.  If you
	' are not sure if the callback blocks for a long time, then first call 
	' IOThreadScheduler.ScheduleCallback to get to a "safe" thread.
	Friend Delegate Sub ItemDequeuedCallback()

	''' <summary>
	''' Handles asynchronous interactions between producers and consumers. 
	''' Producers can dispatch available data to the input queue, 
	''' where it is dispatched to a waiting consumer or stored until a
	''' consumer becomes available. Consumers can synchronously or asynchronously
	''' request data from the queue, which is returned when data becomes
	''' available.
	''' </summary>
	''' <typeparam name="T">The concrete type of the consumer objects that are waiting for data.</typeparam>
	Friend Class InputQueue(Of T As Class)
		Implements IDisposable
		'Stores items that are waiting to be accessed.
        Private itemQueue1 As ItemQueue

		'Each IQueueReader represents some consumer that is waiting for
		'items to appear in the queue. The readerQueue stores them
		'in an ordered list so consumers get serviced in a FIFO manner.
		Private readerQueue As Queue(Of IQueueReader)

		'Each IQueueWaiter represents some waiter that is waiting for
		'items to appear in the queue.  When any item appears, all
		'waiters are signaled.
		Private waiterList As List(Of IQueueWaiter)


        Private Shared onInvokeDequeuedCallback_local As WaitCallback

        Private Shared onDispatchCallback_local As WaitCallback

        Private Shared completeOutstandingReadersCallback_local As WaitCallback

        Private Shared completeWaitersFalseCallback_local As WaitCallback

        Private Shared completeWaitersTrueCallback_local As WaitCallback

        'Represents the current state of the InputQueue
        'as it transitions through its lifecycle.

        Private queueState_local As QueueState
        Private Enum QueueState
            Open
            Shutdown
            Closed
        End Enum

        Public Sub New()
            Me.itemQueue1 = New ItemQueue()
            Me.readerQueue = New Queue(Of IQueueReader)()
            Me.waiterList = New List(Of IQueueWaiter)()
            Me.queueState_local = QueueState.Open
        End Sub

        Public ReadOnly Property PendingCount() As Integer
            Get
                SyncLock ThisLock
                    Return itemQueue1.ItemCount
                End SyncLock
            End Get
        End Property

        Private ReadOnly Property ThisLock() As Object
            Get
                Return itemQueue1
            End Get
        End Property

        Public Function BeginDequeue(ByVal timeout As TimeSpan, ByVal callback As AsyncCallback,
                                     ByVal state As Object) As IAsyncResult
            Dim item As Item = Nothing

            SyncLock ThisLock
                If queueState_local = QueueState.Open Then
                    If itemQueue1.HasAvailableItem Then
                        item = itemQueue1.DequeueAvailableItem()
                    Else
                        Dim reader As New AsyncQueueReader(Me, timeout, callback, state)
                        readerQueue.Enqueue(reader)
                        Return reader
                    End If
                ElseIf queueState_local = QueueState.Shutdown Then
                    If itemQueue1.HasAvailableItem Then
                        item = itemQueue1.DequeueAvailableItem()
                    ElseIf itemQueue1.HasAnyItem Then
                        Dim reader As New AsyncQueueReader(Me, timeout, callback, state)
                        readerQueue.Enqueue(reader)
                        Return reader
                    End If
                End If
            End SyncLock

            InvokeDequeuedCallback(item.DequeuedCallback)
            Return New TypedCompletedAsyncResult(Of T)(item.GetValue(), callback, state)
        End Function

        Public Function BeginWaitForItem(ByVal timeout As TimeSpan, ByVal callback As AsyncCallback,
                                         ByVal state As Object) As IAsyncResult
            SyncLock ThisLock
                If queueState_local = QueueState.Open Then
                    If Not itemQueue1.HasAvailableItem Then
                        Dim waiter As New AsyncQueueWaiter(timeout, callback, state)
                        waiterList.Add(waiter)
                        Return waiter
                    End If
                ElseIf queueState_local = QueueState.Shutdown Then
                    If (Not itemQueue1.HasAvailableItem) AndAlso itemQueue1.HasAnyItem Then
                        Dim waiter As New AsyncQueueWaiter(timeout, callback, state)
                        waiterList.Add(waiter)
                        Return waiter
                    End If
                End If
            End SyncLock

            Return New TypedCompletedAsyncResult(Of Boolean)(True, callback, state)
        End Function

        Private Shared Sub CompleteOutstandingReadersCallback(ByVal state As Object)
            Dim outstandingReaders() As IQueueReader = CType(state, IQueueReader())

            For i = 0 To outstandingReaders.Length - 1
                outstandingReaders(i).Set(Nothing)
            Next i
        End Sub

        Private Shared Sub CompleteWaitersFalseCallback(ByVal state As Object)
            CompleteWaiters(False, CType(state, IQueueWaiter()))
        End Sub

        Private Shared Sub CompleteWaitersTrueCallback(ByVal state As Object)
            CompleteWaiters(True, CType(state, IQueueWaiter()))
        End Sub

        Private Shared Sub CompleteWaiters(ByVal itemAvailable As Boolean, ByVal waiters() As IQueueWaiter)
            For i = 0 To waiters.Length - 1
                waiters(i).Set(itemAvailable)
            Next i
        End Sub

        Private Shared Sub CompleteWaitersLater(ByVal itemAvailable As Boolean, ByVal waiters() As IQueueWaiter)
            If itemAvailable Then
                If completeWaitersTrueCallback_local Is Nothing Then
                    completeWaitersTrueCallback_local = New WaitCallback(AddressOf CompleteWaitersTrueCallback)
                End If

                ThreadPool.QueueUserWorkItem(completeWaitersTrueCallback_local, waiters)
            Else
                If completeWaitersFalseCallback_local Is Nothing Then
                    completeWaitersFalseCallback_local = New WaitCallback(AddressOf CompleteWaitersFalseCallback)
                End If

                ThreadPool.QueueUserWorkItem(completeWaitersFalseCallback_local, waiters)
            End If
        End Sub

        Private Sub GetWaiters(<System.Runtime.InteropServices.Out()> ByRef waiters() As IQueueWaiter)
            If waiterList.Count > 0 Then
                waiters = waiterList.ToArray()
                waiterList.Clear()
            Else
                waiters = Nothing
            End If
        End Sub

        Public Sub Close()
            CType(Me, IDisposable).Dispose()
        End Sub

        Public Sub Shutdown()
            Dim outstandingReaders() As IQueueReader = Nothing
            SyncLock ThisLock
                If queueState_local = QueueState.Shutdown Then
                    Return
                End If

                If queueState_local = QueueState.Closed Then
                    Return
                End If

                Me.queueState_local = QueueState.Shutdown

                If readerQueue.Count > 0 AndAlso Me.itemQueue1.ItemCount = 0 Then
                    outstandingReaders = New IQueueReader(readerQueue.Count - 1) {}
                    readerQueue.CopyTo(outstandingReaders, 0)
                    readerQueue.Clear()
                End If
            End SyncLock

            If outstandingReaders IsNot Nothing Then
                For i = 0 To outstandingReaders.Length - 1
                    outstandingReaders(i).Set(New Item(CType(Nothing, Exception), Nothing))
                Next i
            End If
        End Sub

        Public Function Dequeue(ByVal timeout As TimeSpan) As T
            Dim value As T = Nothing

            If Not Me.Dequeue(timeout, value) Then
                Throw New TimeoutException(String.Format("Dequeue timed out in {0}.", timeout))
            End If

            Return value
        End Function

        Public Function Dequeue(ByVal timeout As TimeSpan,
                                <System.Runtime.InteropServices.Out()> ByRef value As T) As Boolean
            Dim reader As WaitQueueReader = Nothing
            Dim item As New Item()

            SyncLock ThisLock
                If queueState_local = QueueState.Open Then
                    If itemQueue1.HasAvailableItem Then
                        item = itemQueue1.DequeueAvailableItem()
                    Else
                        reader = New WaitQueueReader(Me)
                        readerQueue.Enqueue(reader)
                    End If
                ElseIf queueState_local = QueueState.Shutdown Then
                    If itemQueue1.HasAvailableItem Then
                        item = itemQueue1.DequeueAvailableItem()
                    ElseIf itemQueue1.HasAnyItem Then
                        reader = New WaitQueueReader(Me)
                        readerQueue.Enqueue(reader)
                    Else
                        value = Nothing
                        Return True
                    End If
                Else ' queueState == QueueState.Closed
                    value = Nothing
                    Return True
                End If
            End SyncLock

            If reader IsNot Nothing Then
                Return reader.Wait(timeout, value)
            Else
                InvokeDequeuedCallback(item.DequeuedCallback)
                value = item.GetValue()
                Return True
            End If
        End Function

        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)

            GC.SuppressFinalize(Me)
        End Sub

        Protected Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
                Dim dispose = False

                SyncLock ThisLock
                    If queueState_local <> QueueState.Closed Then
                        queueState_local = QueueState.Closed
                        dispose = True
                    End If
                End SyncLock

                If dispose Then
                    Do While readerQueue.Count > 0
                        Dim reader As IQueueReader = readerQueue.Dequeue()
                        reader.Set(Nothing)
                    Loop

                    Do While itemQueue1.HasAnyItem
                        Dim item As Item = itemQueue1.DequeueAnyItem()
                        item.Dispose()
                        InvokeDequeuedCallback(item.DequeuedCallback)
                    Loop
                End If
            End If
        End Sub

        Public Sub Dispatch()
            Dim reader As IQueueReader = Nothing
            Dim item As New Item()
            Dim outstandingReaders() As IQueueReader = Nothing
            Dim waiters() As IQueueWaiter = Nothing
            Dim itemAvailable = True

            SyncLock ThisLock
                itemAvailable = Not ((queueState_local = QueueState.Closed) OrElse (queueState_local = QueueState.Shutdown))
                Me.GetWaiters(waiters)

                If queueState_local <> QueueState.Closed Then
                    itemQueue1.MakePendingItemAvailable()

                    If readerQueue.Count > 0 Then
                        item = itemQueue1.DequeueAvailableItem()
                        reader = readerQueue.Dequeue()

                        If queueState_local = QueueState.Shutdown AndAlso readerQueue.Count > 0 AndAlso itemQueue1.ItemCount = 0 Then
                            outstandingReaders = New IQueueReader(readerQueue.Count - 1) {}
                            readerQueue.CopyTo(outstandingReaders, 0)
                            readerQueue.Clear()

                            itemAvailable = False
                        End If
                    End If
                End If
            End SyncLock

            If outstandingReaders IsNot Nothing Then
                If completeOutstandingReadersCallback_local Is Nothing Then
                    completeOutstandingReadersCallback_local = New WaitCallback(AddressOf CompleteOutstandingReadersCallback)
                End If

                ThreadPool.QueueUserWorkItem(completeOutstandingReadersCallback_local, outstandingReaders)
            End If

            If waiters IsNot Nothing Then
                CompleteWaitersLater(itemAvailable, waiters)
            End If

            If reader IsNot Nothing Then
                InvokeDequeuedCallback(item.DequeuedCallback)
                reader.Set(item)
            End If
        End Sub

        'Ends an asynchronous Dequeue operation.
        Public Function EndDequeue(ByVal result As IAsyncResult) As T
            Dim value As T = Nothing

            If Not Me.EndDequeue(result, value) Then
                Throw New TimeoutException("Asynchronous Dequeue operation timed out.")
            End If

            Return value
        End Function

        Public Function EndDequeue(ByVal result As IAsyncResult,
                                   <System.Runtime.InteropServices.Out()> ByRef value As T) As Boolean
            Dim typedResult As TypedCompletedAsyncResult(Of T) = TryCast(result, TypedCompletedAsyncResult(Of T))

            If typedResult IsNot Nothing Then
                value = TypedCompletedAsyncResult(Of T).End(result)
                Return True
            End If

            Return AsyncQueueReader.End(result, value)
        End Function

        Public Function EndWaitForItem(ByVal result As IAsyncResult) As Boolean
            Dim typedResult As TypedCompletedAsyncResult(Of Boolean) = TryCast(result, TypedCompletedAsyncResult(Of Boolean))
            If typedResult IsNot Nothing Then
                Return TypedCompletedAsyncResult(Of Boolean).End(result)
            End If

            Return AsyncQueueWaiter.End(result)
        End Function

        Public Sub EnqueueAndDispatch(ByVal item As T)
            EnqueueAndDispatch(item, Nothing)
        End Sub

        Public Sub EnqueueAndDispatch(ByVal item As T, ByVal dequeuedCallback As ItemDequeuedCallback)
            EnqueueAndDispatch(item, dequeuedCallback, True)
        End Sub

        Public Sub EnqueueAndDispatch(ByVal exception As Exception, ByVal dequeuedCallback As ItemDequeuedCallback,
                                      ByVal canDispatchOnThisThread As Boolean)
            EnqueueAndDispatch(New Item(exception, dequeuedCallback), canDispatchOnThisThread)
        End Sub

        Public Sub EnqueueAndDispatch(ByVal item As T, ByVal dequeuedCallback As ItemDequeuedCallback,
                                      ByVal canDispatchOnThisThread As Boolean)
            EnqueueAndDispatch(New Item(item, dequeuedCallback), canDispatchOnThisThread)
        End Sub

        Private Sub EnqueueAndDispatch(ByVal item As Item, ByVal canDispatchOnThisThread As Boolean)
            Dim disposeItem = False
            Dim reader As IQueueReader = Nothing
            Dim dispatchLater = False
            Dim waiters() As IQueueWaiter = Nothing
            Dim itemAvailable = True

            SyncLock ThisLock
                itemAvailable = Not ((queueState_local = QueueState.Closed) OrElse (queueState_local = QueueState.Shutdown))
                Me.GetWaiters(waiters)

                If queueState_local = QueueState.Open Then
                    If canDispatchOnThisThread Then
                        If readerQueue.Count = 0 Then
                            itemQueue1.EnqueueAvailableItem(item)
                        Else
                            reader = readerQueue.Dequeue()
                        End If
                    Else
                        If readerQueue.Count = 0 Then
                            itemQueue1.EnqueueAvailableItem(item)
                        Else
                            itemQueue1.EnqueuePendingItem(item)
                            dispatchLater = True
                        End If
                    End If
                Else ' queueState == QueueState.Closed || queueState == QueueState.Shutdown
                    disposeItem = True
                End If
            End SyncLock

            If waiters IsNot Nothing Then
                If canDispatchOnThisThread Then
                    CompleteWaiters(itemAvailable, waiters)
                Else
                    CompleteWaitersLater(itemAvailable, waiters)
                End If
            End If

            If reader IsNot Nothing Then
                InvokeDequeuedCallback(item.DequeuedCallback)
                reader.Set(item)
            End If

            If dispatchLater Then
                If onDispatchCallback_local Is Nothing Then
                    onDispatchCallback_local = New WaitCallback(AddressOf OnDispatchCallback)
                End If

                ThreadPool.QueueUserWorkItem(onDispatchCallback_local, Me)
            ElseIf disposeItem Then
                InvokeDequeuedCallback(item.DequeuedCallback)
                item.Dispose()
            End If
        End Sub

        Public Function EnqueueWithoutDispatch(ByVal item As T, ByVal dequeuedCallback As ItemDequeuedCallback) As Boolean
            Return EnqueueWithoutDispatch(New Item(item, dequeuedCallback))
        End Function

        Public Function EnqueueWithoutDispatch(ByVal exception As Exception,
                                               ByVal dequeuedCallback As ItemDequeuedCallback) As Boolean
            Return EnqueueWithoutDispatch(New Item(exception, dequeuedCallback))
        End Function

        ' This will not block, however, Dispatch() must be called later if this function
        ' returns true.
        Private Function EnqueueWithoutDispatch(ByVal item As Item) As Boolean
            SyncLock ThisLock
                ' Open
                If queueState_local <> QueueState.Closed AndAlso queueState_local <> QueueState.Shutdown Then
                    If readerQueue.Count = 0 Then
                        itemQueue1.EnqueueAvailableItem(item)
                        Return False
                    Else
                        itemQueue1.EnqueuePendingItem(item)
                        Return True
                    End If
                End If
            End SyncLock

            item.Dispose()
            InvokeDequeuedCallbackLater(item.DequeuedCallback)
            Return False
        End Function

        Private Shared Sub OnDispatchCallback(ByVal state As Object)
            CType(state, InputQueue(Of T)).Dispatch()
        End Sub

        Private Shared Sub InvokeDequeuedCallbackLater(ByVal dequeuedCallback As ItemDequeuedCallback)
            If dequeuedCallback IsNot Nothing Then
                If onInvokeDequeuedCallback_local Is Nothing Then
                    onInvokeDequeuedCallback_local = AddressOf OnInvokeDequeuedCallback
                End If

                ThreadPool.QueueUserWorkItem(onInvokeDequeuedCallback_local, dequeuedCallback)
            End If
        End Sub

        Private Shared Sub InvokeDequeuedCallback(ByVal dequeuedCallback As ItemDequeuedCallback)
            If dequeuedCallback IsNot Nothing Then
                dequeuedCallback()
            End If
        End Sub

        Private Shared Sub OnInvokeDequeuedCallback(ByVal state As Object)
            Dim dequeuedCallback As ItemDequeuedCallback = CType(state, ItemDequeuedCallback)
            dequeuedCallback()
        End Sub

        Private Function RemoveReader(ByVal reader As IQueueReader) As Boolean
            SyncLock ThisLock
                If queueState_local = QueueState.Open OrElse queueState_local = QueueState.Shutdown Then
                    Dim removed = False

                    For i = readerQueue.Count To 1 Step -1
                        Dim temp As IQueueReader = readerQueue.Dequeue()
                        If Object.ReferenceEquals(temp, reader) Then
                            removed = True
                        Else
                            readerQueue.Enqueue(temp)
                        End If
                    Next i

                    Return removed
                End If
            End SyncLock

            Return False
        End Function

        Public Function WaitForItem(ByVal timeout As TimeSpan) As Boolean
            Dim waiter As WaitQueueWaiter = Nothing
            Dim itemAvailable = False

            SyncLock ThisLock
                If queueState_local = QueueState.Open Then
                    If itemQueue1.HasAvailableItem Then
                        itemAvailable = True
                    Else
                        waiter = New WaitQueueWaiter()
                        waiterList.Add(waiter)
                    End If
                ElseIf queueState_local = QueueState.Shutdown Then
                    If itemQueue1.HasAvailableItem Then
                        itemAvailable = True
                    ElseIf itemQueue1.HasAnyItem Then
                        waiter = New WaitQueueWaiter()
                        waiterList.Add(waiter)
                    Else
                        Return False
                    End If
                Else ' queueState == QueueState.Closed
                    Return True
                End If
            End SyncLock

            If waiter IsNot Nothing Then
                Return waiter.Wait(timeout)
            Else
                Return itemAvailable
            End If
        End Function

        Private Interface IQueueReader
            Sub [Set](ByVal item As Item)
        End Interface

        Private Interface IQueueWaiter
            Sub [Set](ByVal itemAvailable As Boolean)
        End Interface

        Private Class WaitQueueReader
            Implements IQueueReader

            Private exception As Exception
            Private inputQueue As InputQueue(Of T)
            Private item As T
            Private waitEvent As ManualResetEvent

            Private thisLock_local As New Object()

            Public Sub New(ByVal inputQueue As InputQueue(Of T))
                Me.inputQueue = inputQueue
                waitEvent = New ManualResetEvent(False)
            End Sub

            Private ReadOnly Property ThisLock() As Object
                Get
                    Return Me.thisLock_local
                End Get
            End Property

            Public Sub [Set](ByVal item As Item) Implements inputQueue(Of T).IQueueReader.Set
                SyncLock ThisLock
                    If Me.item IsNot Nothing OrElse Me.exception IsNot Nothing Then
                        Throw New Exception("Internal Error")
                    End If

                    Me.exception = item.Exception
                    Me.item = item.Value
                    waitEvent.Set()
                End SyncLock
            End Sub

            Public Function Wait(ByVal timeout As TimeSpan,
                                 <System.Runtime.InteropServices.Out()> ByRef value As T) As Boolean
                Dim isSafeToClose As Boolean = False
                Try
                    If timeout = TimeSpan.MaxValue Then
                        waitEvent.WaitOne()
                    ElseIf Not waitEvent.WaitOne(timeout, False) Then
                        If Me.inputQueue.RemoveReader(Me) Then
                            value = Nothing
                            isSafeToClose = True
                            Return False
                        Else
                            waitEvent.WaitOne()
                        End If
                    End If

                    isSafeToClose = True
                Finally
                    If isSafeToClose Then
                        waitEvent.Close()
                    End If
                End Try

                value = item
                Return True
            End Function
        End Class

        Private Class AsyncQueueReader
            Inherits AsyncResult
            Implements IQueueReader


            Private Shared timerCallback_local As New TimerCallback(AddressOf AsyncQueueReader.TimerCallback)

            Private expired As Boolean
            Private inputQueue As InputQueue(Of T)
            Private item As T
            Private timer As Timer

            Public Sub New(ByVal inputQueue As InputQueue(Of T), ByVal timeout As TimeSpan,
                           ByVal callback As AsyncCallback, ByVal state As Object)
                MyBase.New(callback, state)
                Me.inputQueue = inputQueue
                If timeout <> TimeSpan.MaxValue Then
                    Me.timer = New Timer(timerCallback_local, Me, timeout, TimeSpan.FromMilliseconds(-1))
                End If
            End Sub

            Public Overloads Shared Function [End](ByVal result As IAsyncResult,
                                                   <System.Runtime.InteropServices.Out()> ByRef value As T) As Boolean
                Dim readerResult As AsyncQueueReader = AsyncResult.End(Of AsyncQueueReader)(result)

                If readerResult.expired Then
                    value = Nothing
                    Return False
                Else
                    value = readerResult.item
                    Return True
                End If
            End Function

            Private Shared Sub TimerCallback(ByVal state As Object)
                Dim thisPtr As AsyncQueueReader = CType(state, AsyncQueueReader)
                If thisPtr.inputQueue.RemoveReader(thisPtr) Then
                    thisPtr.expired = True
                    thisPtr.Complete(False)
                End If
            End Sub

            Public Sub [Set](ByVal item As Item) Implements inputQueue(Of T).IQueueReader.Set
                Me.item = item.Value
                If Me.timer IsNot Nothing Then
                    Me.timer.Change(-1, -1)
                End If
                Complete(False, item.Exception)
            End Sub
        End Class

        Private Structure Item

            Private value_local As T

            Private exception_local As Exception

            Private dequeuedCallback_local As ItemDequeuedCallback

            Public Sub New(ByVal value As T, ByVal dequeuedCallback As ItemDequeuedCallback)
                Me.New(value, Nothing, dequeuedCallback)
            End Sub

            Public Sub New(ByVal exception As Exception, ByVal dequeuedCallback As ItemDequeuedCallback)
                Me.New(Nothing, exception, dequeuedCallback)
            End Sub

            Private Sub New(ByVal value As T, ByVal exception As Exception, ByVal dequeuedCallback As ItemDequeuedCallback)
                Me.value_local = value
                Me.exception_local = exception
                Me.dequeuedCallback_local = dequeuedCallback
            End Sub

            Public ReadOnly Property Exception() As Exception
                Get
                    Return Me.exception_local
                End Get
            End Property

            Public ReadOnly Property Value() As T
                Get
                    Return value_local
                End Get
            End Property

            Public ReadOnly Property DequeuedCallback() As ItemDequeuedCallback
                Get
                    Return dequeuedCallback_local
                End Get
            End Property

            Public Sub Dispose()
                If value_local IsNot Nothing Then
                    If TypeOf value_local Is IDisposable Then
                        CType(value_local, IDisposable).Dispose()
                    ElseIf TypeOf value_local Is ICommunicationObject Then
                        CType(value_local, ICommunicationObject).Abort()
                    End If
                End If
            End Sub

            Public Function GetValue() As T
                If Me.exception_local IsNot Nothing Then
                    Throw Me.exception_local
                End If

                Return Me.value_local
            End Function
        End Structure

        Private Class WaitQueueWaiter
            Implements IQueueWaiter

            Private itemAvailable As Boolean
            Private waitEvent As ManualResetEvent

            Private thisLock_local As New Object()

            Public Sub New()
                waitEvent = New ManualResetEvent(False)
            End Sub

            Private ReadOnly Property ThisLock() As Object
                Get
                    Return Me.thisLock_local
                End Get
            End Property

            Public Sub [Set](ByVal itemAvailable As Boolean) Implements InputQueue(Of T).IQueueWaiter.Set
                SyncLock ThisLock
                    Me.itemAvailable = itemAvailable
                    waitEvent.Set()
                End SyncLock
            End Sub

            Public Function Wait(ByVal timeout As TimeSpan) As Boolean
                If timeout = TimeSpan.MaxValue Then
                    waitEvent.WaitOne()
                ElseIf Not waitEvent.WaitOne(timeout, False) Then
                    Return False
                End If

                Return Me.itemAvailable
            End Function
        End Class

        Private Class AsyncQueueWaiter
            Inherits AsyncResult
            Implements IQueueWaiter


            Private Shared timerCallback_local As New TimerCallback(AddressOf AsyncQueueWaiter.TimerCallback)
            Private timer As Timer
            Private itemAvailable As Boolean

            Private thisLock_local As New Object()

            Public Sub New(ByVal timeout As TimeSpan, ByVal callback As AsyncCallback, ByVal state As Object)
                MyBase.New(callback, state)
                If timeout <> TimeSpan.MaxValue Then
                    Me.timer = New Timer(timerCallback_local, Me, timeout, TimeSpan.FromMilliseconds(-1))
                End If
            End Sub

            Private ReadOnly Property ThisLock() As Object
                Get
                    Return Me.thisLock_local
                End Get
            End Property

            Public Overloads Shared Function [End](ByVal result As IAsyncResult) As Boolean
                Dim waiterResult As AsyncQueueWaiter = AsyncResult.End(Of AsyncQueueWaiter)(result)
                Return waiterResult.itemAvailable
            End Function

            Private Shared Sub TimerCallback(ByVal state As Object)
                Dim thisPtr As AsyncQueueWaiter = CType(state, AsyncQueueWaiter)
                thisPtr.Complete(False)
            End Sub

            Public Sub [Set](ByVal itemAvailable As Boolean) Implements InputQueue(Of T).IQueueWaiter.Set
                Dim timely As Boolean

                SyncLock ThisLock
                    timely = (Me.timer Is Nothing) OrElse Me.timer.Change(-1, -1)
                    Me.itemAvailable = itemAvailable
                End SyncLock

                If timely Then
                    Complete(False)
                End If
            End Sub
        End Class

		Private Class ItemQueue
			Private items() As Item
			Private head As Integer
			Private pendingCount As Integer
			Private totalCount As Integer

			Public Sub New()
				items = New Item(0){}
			End Sub

			Public Function DequeueAvailableItem() As Item
				If totalCount = pendingCount Then
					Throw New Exception("Internal Error")
				End If
				Return DequeueItemCore()
			End Function

			Public Function DequeueAnyItem() As Item
				If pendingCount = totalCount Then
					pendingCount -= 1
				End If
				Return DequeueItemCore()
			End Function

			Private Sub EnqueueItemCore(ByVal item As Item)
				If totalCount = items.Length Then
					Dim newItems(items.Length * 2 - 1) As Item
                    Dim i = 0
					Do While i < totalCount
						newItems(i) = items((head + i) Mod items.Length)
						i += 1
					Loop
					head = 0
					items = newItems
				End If
                Dim tail = (head + totalCount) Mod items.Length
				items(tail) = item
				totalCount += 1
			End Sub

			Private Function DequeueItemCore() As Item
				If totalCount = 0 Then
					Throw New Exception("Internal Error")
				End If
				Dim item As Item = items(head)
				items(head) = New Item()
				totalCount -= 1
				head = (head + 1) Mod items.Length
				Return item
			End Function

			Public Sub EnqueuePendingItem(ByVal item As Item)
				EnqueueItemCore(item)
				pendingCount += 1
			End Sub

			Public Sub EnqueueAvailableItem(ByVal item As Item)
				EnqueueItemCore(item)
			End Sub

			Public Sub MakePendingItemAvailable()
				If pendingCount = 0 Then
					Throw New Exception("Internal Error")
				End If
				pendingCount -= 1
			End Sub

			Public ReadOnly Property HasAvailableItem() As Boolean
				Get
					Return totalCount > pendingCount
				End Get
			End Property

			Public ReadOnly Property HasAnyItem() As Boolean
				Get
					Return totalCount > 0
				End Get
			End Property

			Public ReadOnly Property ItemCount() As Integer
				Get
					Return totalCount
				End Get
			End Property
		End Class
	End Class
End Namespace
