' Copyright (c) Microsoft Corporation.  All Rights Reserved.

Imports System
Imports System.Diagnostics
Imports System.IO
Imports System.Globalization

Namespace Microsoft.Samples.ServiceModel

    Public Class CircularTraceListener
        Inherits XmlWriterTraceListener

        Shared m_stream As CircularStream = Nothing
        Private MaxQuotaInitialized As Boolean = False
        Const FileQuotaAttribute As String = "maxFileSizeKB"
        Const DefaultMaxQuota As Long = 1000
        Const DefaultTraceFile As String = "E2ETraces.svclog"

#Region "Member Functions"

        Private ReadOnly Property MaxQuotaSize() As Long

            'Get the MaxQuotaSize from configuration file
            'Set to Default Value if there are any problems

            Get

                Dim MaxFileQuota As Long = 0
                If Not Me.MaxQuotaInitialized Then

                    Try

                        Dim MaxQuotaOption As String = Me.Attributes(CircularTraceListener.FileQuotaAttribute)
                        If MaxQuotaOption Is Nothing Then

                            MaxFileQuota = DefaultMaxQuota

                        Else

                            MaxFileQuota = Integer.Parse(MaxQuotaOption, CultureInfo.InvariantCulture)

                        End If

                    Catch generatedExceptionName As Exception

                        MaxFileQuota = DefaultMaxQuota

                    Finally

                        Me.MaxQuotaInitialized = True

                    End Try

                End If

                If MaxFileQuota <= 0 Then

                    MaxFileQuota = DefaultMaxQuota

                End If

                'MaxFileQuota is in KB in the configuration file, convert to bytes

                MaxFileQuota = MaxFileQuota * 1024
                Return MaxFileQuota

            End Get

        End Property

        Private Sub DetermineOverQuota()

            'Set the MaxQuota on the stream if it hasn't been done

            If Not Me.MaxQuotaInitialized Then

                m_stream.MaxQuotaSize = Me.MaxQuotaSize

            End If

            'If we're past the Quota, flush, then switch files

            If m_stream.IsOverQuota Then

                MyBase.Flush()
                m_stream.SwitchFiles()

            End If

        End Sub

#End Region

#Region "XmlWriterTraceListener Functions"

        Public Sub New(ByRef stream As CircularStream)
            MyBase.New(stream)

        End Sub

        Public Sub New(ByVal file As String)
            MyBase.New(file)
            m_stream = New CircularStream(file)
            Dim writer As StreamWriter = New StreamWriter(m_stream)
            MyBase.Writer = writer
        End Sub

        Protected Overloads Overrides Function GetSupportedAttributes() As String()

            Return New String() {CircularTraceListener.FileQuotaAttribute}

        End Function

        Public Overloads Overrides Sub TraceData(ByVal eventCache As TraceEventCache, ByVal source As String, ByVal eventType As TraceEventType, ByVal id As Integer, ByVal data As Object)

            DetermineOverQuota()
            MyBase.TraceData(eventCache, source, eventType, id, data)

        End Sub

        Public Overloads Overrides Sub TraceEvent(ByVal eventCache As TraceEventCache, ByVal source As String, ByVal eventType As TraceEventType, ByVal id As Integer)

            DetermineOverQuota()
            MyBase.TraceEvent(eventCache, source, eventType, id)

        End Sub

        Public Overloads Overrides Sub TraceData(ByVal eventCache As TraceEventCache, ByVal source As String, ByVal eventType As TraceEventType, ByVal id As Integer, ByVal ParamArray data As Object())

            DetermineOverQuota()
            MyBase.TraceData(eventCache, source, eventType, id, data)

        End Sub

        Public Overloads Overrides Sub TraceEvent(ByVal eventCache As TraceEventCache, ByVal source As String, ByVal eventType As TraceEventType, ByVal id As Integer, ByVal format As String, ByVal ParamArray args As Object())

            DetermineOverQuota()
            MyBase.TraceEvent(eventCache, source, eventType, id, format, args)

        End Sub

        Public Overloads Overrides Sub TraceEvent(ByVal eventCache As TraceEventCache, ByVal source As String, ByVal eventType As TraceEventType, ByVal id As Integer, ByVal message As String)

            DetermineOverQuota()
            MyBase.TraceEvent(eventCache, source, eventType, id, message)

        End Sub

        Public Overloads Overrides Sub TraceTransfer(ByVal eventCache As TraceEventCache, ByVal source As String, ByVal id As Integer, ByVal message As String, ByVal relatedActivityId As Guid)

            DetermineOverQuota()
            MyBase.TraceTransfer(eventCache, source, id, message, relatedActivityId)

        End Sub

#End Region

    End Class

    Public Class CircularStream
        Inherits System.IO.Stream

        Private FStream As FileStream() = Nothing
        Private FPath As String() = Nothing
        Private DataWritten As Long = 0
        Private FileQuota As Long = 0
        Private CurrentFile As Integer = 0
        Private stringWritten As String = String.Empty

        Public Sub New(ByVal FileName As String)

            'Handle all exceptions within this class, since tracing shouldn't crash a service

            'Add 00 and 01 to FileNames and open streams

            Try

                Dim filePath As String = Path.GetDirectoryName(FileName)
                Dim fileBase As String = Path.GetFileNameWithoutExtension(FileName)
                Dim fileExt As String = Path.GetExtension(FileName)

                If String.IsNullOrEmpty(filePath) Then

                    filePath = AppDomain.CurrentDomain.BaseDirectory

                End If

                FPath = New String(2) {}
                FPath(0) = Path.Combine(filePath, fileBase + "00" + fileExt)
                FPath(1) = Path.Combine(filePath, fileBase + "01" + fileExt)

                FStream = New FileStream(2) {}
                FStream(0) = New FileStream(FPath(0), FileMode.Create)

            Catch

            End Try

        End Sub

        Public Property MaxQuotaSize() As Long

            Get

                Return FileQuota

            End Get
            Set(ByVal value As Long)

                FileQuota = value

            End Set

        End Property

        Public Sub SwitchFiles()

            Try

                'Close current file, open next file (deleting its contents)

                DataWritten = 0
                FStream(CurrentFile).Close()

                CurrentFile = (CurrentFile + 1) Mod 2

                FStream(CurrentFile) = New FileStream(FPath(CurrentFile), FileMode.Create)

            Catch generatedExceptionName As Exception

            End Try

        End Sub

        Public ReadOnly Property IsOverQuota() As Boolean

            Get

                Return (DataWritten >= FileQuota)

            End Get

        End Property

        Public Overloads Overrides ReadOnly Property CanRead() As Boolean

            Get

                Try

                    Return FStream(CurrentFile).CanRead

                Catch generatedExceptionName As Exception

                    Return True

                End Try

            End Get

        End Property

        Public Overloads Overrides ReadOnly Property CanSeek() As Boolean

            Get

                Try

                    Return FStream(CurrentFile).CanSeek

                Catch generatedExceptionName As Exception

                    Return False

                End Try

            End Get

        End Property

        Public Overloads Overrides ReadOnly Property Length() As Long

            Get

                Try

                    Return FStream(CurrentFile).Length

                Catch generatedExceptionName As Exception

                    Return -1

                End Try

            End Get

        End Property

        Public Overloads Overrides Property Position() As Long

            Get

                Try

                    Return FStream(CurrentFile).Position

                Catch generatedExceptionName As Exception

                    Return -1

                End Try

            End Get
            Set(ByVal value As Long)

                Try

                    FStream(CurrentFile).Position = Position

                Catch generatedExceptionName As Exception

                End Try

            End Set

        End Property

        Public Overloads Overrides ReadOnly Property CanWrite() As Boolean

            Get

                Try

                    Return FStream(CurrentFile).CanWrite

                Catch generatedExceptionName As Exception

                    Return True

                End Try

            End Get

        End Property

        Public Overloads Overrides Sub Flush()

            Try

                FStream(CurrentFile).Flush()

            Catch generatedExceptionName As Exception

            End Try

        End Sub

        Public Overloads Overrides Function Seek(ByVal offset As Long, ByVal origin As SeekOrigin) As Long

            Try

                Return FStream(CurrentFile).Seek(offset, origin)

            Catch generatedExceptionName As Exception

                Return -1

            End Try

        End Function

        Public Overloads Overrides Sub SetLength(ByVal value As Long)

            Try

                FStream(CurrentFile).SetLength(value)

            Catch generatedExceptionName As Exception

            End Try

        End Sub

        Public Overloads Overrides Sub Write(ByVal buffer As Byte(), ByVal offset As Integer, ByVal count As Integer)

            Try

                'Write to current file
                FStream(CurrentFile).Write(buffer, offset, count)
                DataWritten += count

            Catch generatedExceptionName As Exception

            End Try

        End Sub

        Public Overloads Overrides Function Read(ByVal buffer As Byte(), ByVal offset As Integer, ByVal count As Integer) As Integer

            Try

                Return FStream(CurrentFile).Read(buffer, offset, count)

            Catch generatedExceptionName As Exception

                Return -1

            End Try

        End Function

        Public Overloads Overrides Sub Close()

            Try

                FStream(CurrentFile).Close()

            Catch generatedExceptionName As Exception

            End Try

        End Sub

    End Class

End Namespace
