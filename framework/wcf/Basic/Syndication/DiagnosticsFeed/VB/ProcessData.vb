'----------------------------------------------------------------
' Copyright (c) Microsoft Corporation.  All rights reserved. 
'----------------------------------------------------------------

Imports System
Imports System.Diagnostics
Imports System.Text
Imports System.Runtime.Serialization

Namespace Microsoft.Samples.DiagnosticsFeed
    <DataContract()> _
    Class ProcessData
        <DataMember()> _
        Private VirtualMemorySize As Long

        <DataMember()> _
        Private PeakVirtualMemorySize As Long

        <DataMember()> _
        Private PeakWorkingSetSize As Long

        Public Sub New(ByVal p As Process)
            Me.VirtualMemorySize = p.VirtualMemorySize64
            Me.PeakVirtualMemorySize = p.PeakVirtualMemorySize64
            Me.PeakWorkingSetSize = p.PeakWorkingSet64
        End Sub

        Public Overloads Overrides Function ToString() As String
            Dim sb As New StringBuilder()
            sb.AppendFormat("Virtual Memory: {0}", Me.VirtualMemorySize)
            sb.Append(Environment.NewLine)
            sb.AppendFormat("PeakVirtualMemorySize: {0}", Me.PeakVirtualMemorySize)
            sb.Append(Environment.NewLine)
            sb.AppendFormat("PeakWorkingSetSize: {0}", Me.PeakWorkingSetSize)
            sb.Append(Environment.NewLine)

            Return sb.ToString()
        End Function
    End Class
End Namespace
