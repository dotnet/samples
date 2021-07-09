Set shell = Wscript.CreateObject( "WScript.Shell" )

If Wscript.Arguments.Count < 1 Then 
   usage = "USAGE: CleanUpVdir.vbs virtual_directory_name" & vbCrLf & vbCrLf 
   usage = usage & "=========" & vbCrLf 
   usage = usage & "ARGUMENTS" & vbCrLf 
   usage = usage & "=========" & vbCrLf 
   usage = usage & "virtual_directory_name   : String that represents the name of the" & vbCrLf 
   usage = usage & "                           virtual directory, in the form parent1/parent2/virtual_directory_name. " & vbCrLf
   usage = usage & "                           Only virtual_directory_name is deleted." & vbCrLf 
   WScript.Echo usage
   Wscript.Quit 
End If

fullVDirName = Wscript.Arguments(0)

' Split vDirName in parent vdir and vdir to create
parentVDirName = ""
vDirName = fullVDirName
If InStr(fullVDirName, "/") > 0 Then
	
	' Strip "/" from beginning of string
	If (InStr(fullVDirName, "/")) = 1 Then
		fullVDirName = Right(fullVDirName, Len(fullVDirName) - 1)
	End If

	' Split
	vDirArray = Split(StrReverse(fullVDirName), "/", 2)
	vDirName = StrReverse(vDirArray(0))
	parentVDirName = StrReverse(vDirArray(1))
End If

If vDirName = "" Then
	shell.Popup "vDirName invalid: " & vDirName, 0, "Error" & " (CleanUpVdir.vbs)", 16 ' 16 = Stop
	WScript.Quit
End If

If VDirExists(fullVDirName) Then

	DeleteVirtualDirectory parentVDirName, vDirName
	
	' Inform user of result
	If Not VDirExists(fullVDirName) Then
	    result = "Deleted '" & "/" & fullVDirName
		shell.Popup result, 0, "Virtual Directory Deleted" & " (CleanUpVdir.vbs)", 64 ' 64 = Information
	End If
Else
	shell.Popup "Virtual directory not found: " & "/" & fullVDirName, 0, "Error" & " (CleanUpVdir.vbs)", 16 ' 16 = Stop
End If

Sub DeleteVirtualDirectory( parentVDirName, vDirName )

	Dim iisName
	If parentVDirName = "" Then
		iisName = "IIS://localhost/W3SVC/1/Root"
	Else
		iisName = "IIS://localhost/W3SVC/1/Root/" & parentVDirName
	End If
	Set objIIS = GetObject(iisName)

	If Err.Number <> 0 Then
		shell.Popup Err.Description, 0, "Error" & " (CleanUpVdir.vbs)", 16 ' 16 = Stop
		WScript.Quit
	End If
		
    objIIS.Delete "IISWebVirtualDir", vDirName
    
    If Err.Number <> 0 Then

      errorString = "Unable to delete existing virtual directory."

      If Err.Description Is Nothing Then
         errorString = errorString & "Error Code: " & Err.Number
      Else
         errorString = errorString & "Description: " & Err.Description
      End If

      shell.Popup errorString, 0, "Error" & " (CleanUpVdir.vbs)", 16 ' 16 = Stop
    End If

End Sub

Function VDirExists( name )
	
	If IsEmpty(name) Or IsNull(name) Or Len(name) = 0 Then
		VDirExists = True
		Exit Function
	End If
	
	On Error Resume Next
	Set objIIS = GetObject( "IIS://localhost/W3SVC/1/Root/" & name )
	If Err.Number = 0 Then    
	    VDirExists = True
	Else
		Err.Clear
		VDirExists = False
	End If
End Function