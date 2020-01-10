using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

class Program
{
    static IEnumerable<XElement> StreamRootChildDoc(string uri)
    {
        using (XmlReader reader = XmlReader.Create(uri))
        {
            reader.MoveToContent();
            
            // Parse the file and return each of the nodes.
            while (!reader.EOF)
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "Child")
                {
                    XElement el = XElement.ReadFrom(reader) as XElement;
                    if (el != null)
                        yield return el;
                }
                else
                {
                    reader.Read();
                }
            }
        }
    }

    static void Main(string[] args)
    {
        IEnumerable<string> grandChildData =
            from el in StreamRootChildDoc("Source.xml")
            where (int)el.Attribute("Key") > 1
            select (string)el.Element("GrandChild");

        foreach (string str in grandChildData)
            Console.WriteLine(str);
    }
}