// *** script for retrieving user defined WMI Object ***
wcf = GetObject("winmgmts:root/ServiceModel");
objSet = wcf.InstancesOf("WMIObject");
customObjectCount = objSet.Count;

WScript.Echo ("");
WScript.Echo (customObjectCount + " WMIObject(s) found.");


var customObjects = new Enumerator (objSet);
while (!customObjects.atEnd())
{
    var WMIObject = customObjects.item();
    
    WScript.Echo("PID:           " + WMIObject.ProcessId);
    WScript.Echo("InstanceId:    " + WMIObject.InstanceId);
    WScript.Echo("WMIInfo:       " + WMIObject.WMIInfo);
    customObjects.moveNext ();
}