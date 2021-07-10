Set shell = Wscript.CreateObject( "WScript.Shell" )

If Wscript.Arguments.Count < 2 Then 
   usage = "USAGE: CleanUpVdir.vbs virtual_directory_name directory_location_to_map" & vbCrLf & vbCrLf 
   usage = usage & "=========" & vbCrLf 
   usage = usage & "YOU DO NOT NEED TO RUN THIS SCRIPT MANUALLY TO RUN THE SAMPLES. " & vbCrLf
   usage = usage & "The Visual Studio projects call it when built." & vbCrLf 
   usage = usage & "=========" & vbCrLf  & vbCrLf
   usage = usage & "ARGUMENTS" & vbCrLf 
   usage = usage & "=========" & vbCrLf 
   usage = usage & "virtual_directory_name   : String that represents the name of the" & vbCrLf 
   usage = usage & "                           virtual directory, in the form parent1/parent2/virtual_directory_name. " & vbCrLf
   usage = usage & "                           Only virtual_directory_name is created." & vbCrLf 
   usage = usage & "directory_location_to_map: Directory to be mapped to the virtual" & vbCrLf 
   usage = usage & "                           directory. Can be relative to the" & vbCrLf 
   usage = usage & "                           current directory." & vbCrLf 
   WScript.Echo usage
   Wscript.Quit 
End If

fullVDirName = Wscript.Arguments(0)
vDirPath = Wscript.Arguments(1)

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

' Get the full path of the directory_location_to_map
Set fso = WScript.CreateObject( "Scripting.FileSystemObject" )
vDirPath = fso.GetFolder( vDirPath ).Path

' Does the parent vdir exist in the in the metabase?
If Not VDirExists(parentVDirName) Then
    shell.Popup "Parent virtual directory does not exist: " & parentVDirName, 0, "Error" & " (CleanUpVdir.vbs)", 16 ' 16 = Stop
    WScript.Quit
End If

' Does vDirName IIS application already exist in the metabase?
On Error Resume Next
If VDirExists(fullVDirName) Then

    ' Check whether the path is as desired
    Set objIIS = GetObject( "IIS://localhost/W3SVC/1/Root/" & fullVDirName )    

    If Err.Number <> 0 Then
        shell.Popup Err.Description, 0, "Error" & " (CleanUpVdir.vbs)", 16 ' 16 = Stop
        WScript.Quit
    End If

    If objIIS.Path = vDirPath Then
        WScript.Quit
    Else
        ' Need to remap virtual directory
        result = 6
        
        ' We could warn, but do not (Just tell them that we mapped a new vdir.)
        'result = shell.Popup( "A virtual directory named " & fullVDirName & " already exists. " & vbCrLf & vbCrLf _
            '& "Would you like it re-mapped for this sample?", 0 ,"Remap Virtual Directory?" & " (CleanUpVdir.vbs)", 4 + 32 )' 4 = YesNo & 32 = Question
            
        If result = 6 Then ' 6 = Yes
            DeleteVirtualDirectory  parentVDirName, vDirName 
        Else
            WScript.Quit
        End If
    End If
End If

'Using IIS Administration object , turn on script/execute permissions and define the virtual directory as an 'in-process application. 
Dim iisName
If parentVDirName = "" Then
    iisName = "IIS://localhost/W3SVC/1/Root"
Else
    iisName = "IIS://localhost/W3SVC/1/Root/" & parentVDirName
End If

Set objIIS = GetObject(iisName)

If Err.Number <> 0 Then
    shell.Popup "Could not access " & iisName & ": " & Err.Description, 0, "Error" & " (CleanUpVdir.vbs)", 16 ' 16 = Stop
    WScript.Quit
End If

Set vDirObj = objIIS.Create( "IISWebVirtualDir", vDirName )

If Err.Number <> 0 Then
    shell.Popup "Could not create " & iisName & "/" & vDirName & ": " & Err.Description, 0, "Error" & " (CleanUpVdir.vbs)", 16 ' 16 = Stop
    WScript.Quit
End If

vDirObj.Path                  = vDirPath
vDirObj.AuthNTLM              = True
vDirObj.AccessRead            = True
vDirObj.AccessWrite           = True 
vDirObj.AccessScript          = True
vDirObj.AccessExecute         = True
vDirObj.AuthAnonymous         = True
'vDirObj.AnonymousUserName     = owner
vDirObj.AnonymousPasswordSync = True
vDirObj.AppFriendlyName       = vDirName & "App"
vDirObj.AppCreate True
vDirObj.SetInfo 

If Err.Number <> 0 Then
    shell.Popup "Could not set properties on " & iisName & "/" & vDirName & ": " & Err.Description, 0, "Error" & " (CleanUpVdir.vbs)", 16 ' 16 = Stop
    WScript.Quit
End If

' Get the name of the account for the anonymous user in IIS
owner = vDirObj.AnonymousUserName

' Change necessary folder permissions using CACLS.exe
aclCmd = "cmd /c echo y| CACLS "
aclCmd = aclCmd & """" & vDirPath & """"
aclCmd = aclCmd & " /E /G " & owner & ":C"
rtc = shell.Run( aclCmd , 0, True )

aclCmd = "cmd /c echo y| CACLS "
aclCmd = aclCmd & """" & vDirPath & """"
aclCmd = aclCmd & " /E /G ""VS Developers"":C"
rtc = shell.Run( aclCmd , 0, True )

If Err.Number <> 0 Then
    shell.Popup "Could not set ACLs on " & iisName & "/" & vDirName & ": " & Err.Description, 0, "Error" & " (CleanUpVdir.vbs)", 16 ' 16 = Stop
    WScript.Quit
Else
    res = "Created '" & "/" & fullVDirName & "' virtual directory mapped to " & vbCrLf & vDirPath
    shell.Popup res, 0, "Virtual Directory Created" & " (CleanUpVdir.vbs)", 64 ' 64 = Information
End If

Sub DeleteVirtualDirectory( parentVDirName, vDirName )

    Dim iisName
    If parentVDirName = "" Then
        iisName = "IIS://localhost/W3SVC/1/Root"
    Else
        iisName = "IIS://localhost/W3SVC/1/Root/" & parentVDirName
    End If
    Set iis = GetObject(iisName)

    If Err.Number <> 0 Then
        shell.Popup Err.Description, 0, "Error" & " (CleanUpVdir.vbs)", 16 ' 16 = Stop
        WScript.Quit
    End If
        
    iis.Delete "IISWebVirtualDir", vDirName
    
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