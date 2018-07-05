// <Snippet1>
using System;
class MainApp 
{
    public static void Main()
    {
            // Use the ProgID localhost\HKEY_CLASSES_ROOT\DirControl.DirList.1.
            string theProgramID ="DirControl.DirList.1"; 
            // Use the server name localhost.
            string theServer="localhost";
            // Make a call to the method to get the type information for the given ProgID.
            Type myType =Type.GetTypeFromProgID(theProgramID,theServer);
            if(myType==null)
            {
                throw new Exception("Invalid ProgID or Server.");
            }
            Console.WriteLine("GUID for ProgID DirControl.DirList.1 is {0}.", myType.GUID);
    }
}
// </Snippet1>
