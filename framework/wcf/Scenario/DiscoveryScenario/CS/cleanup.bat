@echo off

echo -------------------------
echo Clean up certificates
echo -------------------------

echo Clean up certificates
certmgr.exe -del -c -n DiscoverySecureClientCertificate -r LocalMachine -s TrustedPeople
certmgr.exe -del -c -n DiscoverySecureServiceCertificate -r LocalMachine -s TrustedPeople
certmgr.exe -del -c -n DiscoverySecureAnnouncementServiceCertificate -r LocalMachine -s TrustedPeople

certmgr.exe -del -c -n DiscoverySecureClientCertificate -r LocalMachine -s My
certmgr.exe -del -c -n DiscoverySecureServiceCertificate -r LocalMachine -s My
certmgr.exe -del -c -n DiscoverySecureAnnouncementServiceCertificate -r LocalMachine -s My

IF %errorlevel%==0 (
echo -------------------------
echo SUCCESS
echo Certificates were cleaned up successfully.
echo -------------------------
) ELSE (
echo -------------------------
echo ERROR
echo Certificates were not cleaned up successfully.
echo Note that this script must be run with administrative privileges from the VS tools command prompt.
echo -------------------------
)

