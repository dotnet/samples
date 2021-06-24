::Running the Channels Tester with a standard binding from Windows Communication Foundation
CustomChannelsTester.exe /binding:WSHttpBinding /testspec:TestSpec.xml

::Running the Channels Tester with a custom binding created as a sample
::The sample is located at Current\TechnologySamples\Extensibility\Transport\Udp
CustomChannelsTester.exe /binding:SampleProfileUdpBinding /dll:"..\..\..\..\Extensibility\Transport\Udp\CS\UdpTransport\bin\UdpTransport.dll" /testspec:TestSpec.xml

