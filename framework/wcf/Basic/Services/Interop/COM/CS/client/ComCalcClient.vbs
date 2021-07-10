' Copyright (c) Microsoft Corporation.  All Rights Reserved.

'---------------------------------------------------------------
' Typed service moniker example
'---------------------------------------------------------------

' Create a service moniker object using a strongly typed contract
' This references the address, a standard binding type and the
' locally registered COM-visible contract ID
Set typedServiceMoniker = GetObject("service4:address=http://localhost/ServiceModelSamples/service.svc, binding=wsHttpBinding, contract={80CBEDDE-D82C-3BD2-8BE4-C2EBC48EB139}")

' Call the service operations using the moniker object
WScript.Echo "Typed service moniker: 100 + 15.99 = " & typedServiceMoniker.Add(100, 15.99)

Set typedServiceMoniker = nothing


'---------------------------------------------------------------
' WSDL service moniker example
'---------------------------------------------------------------

' Open the WSDL contract file and read it into the wsdlContract string
Const ForReading = 1
Set objFSO = CreateObject("Scripting.FileSystemObject")
Set objFile = objFSO.OpenTextFile("serviceWsdl.xml", ForReading)
wsdlContract = objFile.ReadAll
objFile.Close

' Create a string for the service moniker including the content
' of the WSDL contract file
wsdlMonikerString = "service4:address='http://localhost/ServiceModelSamples/service.svc'"
wsdlMonikerString = wsdlMonikerString + ", binding=WSHttpBinding_ICalculator, bindingNamespace='http://Microsoft.ServiceModel.Samples'"
wsdlMonikerString = wsdlMonikerString + ", wsdl='" & wsdlContract & "'"
wsdlMonikerString = wsdlMonikerString + ", contract=ICalculator, contractNamespace='http://Microsoft.ServiceModel.Samples'"

' Create the service moniker object
Set wsdlServiceMoniker = GetObject(wsdlMonikerString)

' Call the service operations using the moniker object
WScript.Echo "WSDL service moniker: 145 - 76.54 = " & wsdlServiceMoniker.Subtract(145, 76.54)

Set wsdlServiceMoniker = nothing


'---------------------------------------------------------------
' MEX service moniker example
'---------------------------------------------------------------

' Create a string for the service moniker specifying the address
' to retrieve the service metadata from
mexMonikerString = "service4:mexAddress='http://localhost/servicemodelsamples/service.svc/mex'"
mexMonikerString = mexMonikerString + ", address='http://localhost/ServiceModelSamples/service.svc'"
mexMonikerString = mexMonikerString + ", binding=WSHttpBinding_ICalculator, bindingNamespace='http://Microsoft.ServiceModel.Samples'"
mexMonikerString = mexMonikerString + ", contract=ICalculator, contractNamespace='http://Microsoft.ServiceModel.Samples'"

' Create the service moniker object
Set mexServiceMoniker = GetObject(mexMonikerString)

' Call the service operations using the moniker object
WScript.Echo "MEX service moniker: 9 * 81.25 = " & mexServiceMoniker.Multiply(9, 81.25)

Set mexServiceMoniker = nothing