@ECHO OFF

echo --------------------------------------------------------------------
echo           Deleting the tool created by SetupCertTool.bat
echo --------------------------------------------------------------------
echo.

del "%ProgramFiles%\ServiceModelSampleTools\FindPrivateKey.exe"
If ERRORLEVEL 1 goto error01

eCHO %ProgramFiles%\ServiceModelSampleTools\FindPrivateKey.exe has been deleted.  
echo Please delete the %ProgramFiles%\ServiceModelSampleTools directory manually.

echo.
echo ----------------------------------
echo   Cleanup completed successfully
echo ----------------------------------

goto :eof

:error01
echo -----------------------
echo   Error Running Setup
echo -----------------------
echo Try executing this script in a Visual Studio command prompt (running as  
echo administrator) if you have not already.
goto :eof
