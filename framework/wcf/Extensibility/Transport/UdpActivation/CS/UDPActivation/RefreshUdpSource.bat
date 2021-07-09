REM This script is to get common files from the UdpTransport sample.

set UDP_SRC_PATH=%~dp0..\..\..\Udp\CS\UdpTransport

for %%i in (AsyncResult.cs InputQueue.cs) do (
	sd edit %~dp0%%i
	copy /y %UDP_SRC_PATH%\%%i %~dp0%%i
)

for %%i in (SampleProfileUdpBinding.cs SampleProfileUdpBindingConfigurationElement.cs SampleProfileUdpBindingCollectionElement.cs UdpChannelFactory.cs UdpChannelHelpers.cs UdpInputChannel.cs UdpTransportElement.cs) do (
	sd edit %~dp0Channels\%%i
	copy /y %UDP_SRC_PATH%\%%i %~dp0Channels\%%i
	rep "\"soap.udp\"" "\"net.udp\"" %~dp0Channels\%%i
)
