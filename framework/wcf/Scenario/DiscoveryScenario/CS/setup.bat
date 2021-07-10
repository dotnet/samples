@echo off

echo -------------------------
echo Add certificates
echo -------------------------

echo Setting up Client certificate
makecert.exe -sr LocalMachine -ss My -a sha1 -sk DiscoverySecureClient -n CN=DiscoverySecureClientCertificate -sky exchange -pe
certmgr.exe -add -c -n DiscoverySecureClientCertificate -s -r localMachine My -s -r localMachine TrustedPeople

echo Setting up Service certificate
makecert.exe -sr LocalMachine -ss My -a sha1 -sk DiscoverySecureService -n CN=DiscoverySecureServiceCertificate -sky exchange -pe serviceTestCert.cer
certmgr.exe -add -c -n DiscoverySecureServiceCertificate -s -r localMachine My -s -r localMachine TrustedPeople

echo Setting up announcement service certificate
makecert.exe -sr LocalMachine -ss My -a sha1 -sk DiscoverySecureAnnouncementService -n CN=DiscoverySecureAnnouncementServiceCertificate -sky exchange -pe serviceAnnTestCert.cer
certmgr.exe -add -c -n DiscoverySecureAnnouncementServiceCertificate -s -r localMachine My -s -r localMachine TrustedPeople

IF %errorlevel%==0 (
echo -------------------------
echo SUCCESS
echo Certificates were added successfully.
echo To clean up, run CleanUp.bat
echo -------------------------
) ELSE (
echo -------------------------
echo ERROR
echo Certificates were not added successfully. 
echo Note that this script must be run with administrative privileges from the VS tools command prompt.
echo -------------------------
)

