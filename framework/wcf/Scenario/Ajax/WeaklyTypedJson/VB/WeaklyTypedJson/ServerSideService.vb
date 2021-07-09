'----------------------------------------------------------------
' Copyright (c) Microsoft Corporation.  All rights reserved. 
'----------------------------------------------------------------

Imports Microsoft.VisualBasic
Imports System.Runtime.Serialization
Imports System.ServiceModel
Imports System.ServiceModel.Web

Namespace Microsoft.Samples.WeaklyTypedJson

    <ServiceContract()> _
    Public Class ServerSideProfileService
        ' The service returns the MemberProfile complex type

        <WebGet(ResponseFormat:=WebMessageFormat.Json)> _
        Public Function GetMemberProfile() As MemberProfile
            Dim member As New MemberProfile()
            member.personal = New PersonalInfo()

            member.personal.name = "Paul"
            member.personal.age = 23
            member.personal.height = 1.7
            member.personal.isSingle = True
            member.personal.luckyNumbers = New Integer(2) {5, 17, 21}

            member.favoriteBands = New String(1) {"Band ABC", "Band XYZ"}

            Return member
        End Function
    End Class

    <DataContract()> _
    Public Class MemberProfile
        <DataMember()> _
        Public personal As PersonalInfo

        <DataMember()> _
        Public favoriteBands() As String
    End Class

    <DataContract()> _
    Public Class PersonalInfo
        <DataMember()> _
        Public name As String

        <DataMember()> _
        Public age As Integer

        <DataMember()> _
        Public height As Double

        <DataMember()> _
        Public isSingle As Boolean

        <DataMember()> _
        Public luckyNumbers() As Integer
    End Class
End Namespace
