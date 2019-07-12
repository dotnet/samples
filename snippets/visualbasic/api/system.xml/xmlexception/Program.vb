Try
    Dim contacts As XElement = XElement.Parse(
        "<Contacts>  
            <Contact>  
                <Name>Jim Wilson</Name>  
            </Contact>  
         </Contcts>")
    Console.WriteLine(contacts)
Catch e As System.Xml.XmlException
    Console.WriteLine(e.Message)
End Try
