'
' This script uses WMI to get the name of the machine be used as the CN ' for the certificates for WCF security samples.
'
set wmi = Getobject("winmgmts:")
wql = "select * from win32_computersystem"
set results = wmi.execquery(wql)
for each compsys in results
    'check if the machine is in the workgroup or domain
    if compsys.PartOfDomain = 0 or compsys.Domain = compsys.Workgroup then 
        ' only get the name of the machine
        WScript.echo compsys.name
    else
        ' get the fully qualified name of the machine
        n = compsys.name & "." & compsys.domain
        WScript.echo n
    end if
next