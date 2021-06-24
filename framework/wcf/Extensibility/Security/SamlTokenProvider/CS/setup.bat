echo off
set SERVER_NAME=localhost
set USER_NAME=Alice
echo ---------------------------------------------------------------------
echo cleaning up the certificates from previous run
certmgr.exe -del -r LocalMachine -s My -c -n %SERVER_NAME%
certmgr.exe -del -r CurrentUser -s TrustedPeople -c -n %SERVER_NAME%
certmgr.exe -del -r CurrentUser -s My -c -n %USER_NAME%
certmgr.exe -del -r LocalMachine -s TrustedPeople -c -n %USER_NAME%

echo ---------------------------------------------------------------------
echo Server cert setup starting
echo for server: %SERVER_NAME%
echo making server cert
makecert.exe -sr LocalMachine -ss My -a sha1 -n CN=%SERVER_NAME% -sky exchange -pe -sk 974877a3-6154-40e4-b688-25fe8962dc17
certmgr.exe -add -c -n %SERVER_NAME% -s -r localMachine My -s TrustedPeople
echo ---------------------------------------------------------------------
echo ---------------------------------------------------------------------
echo Server cert setup starting
echo for client: %USER_NAME%
echo making server cert
makecert.exe -sr CurrentUser -ss My -a sha1 -n CN=%USER_NAME% -sky exchange -pe -sk 974877a3-6154-40e4-b688-25fe8962dc18
certmgr.exe -add -c -n %USER_NAME% -s My -s -r localMachine TrustedPeople
echo ---------------------------------------------------------------------
