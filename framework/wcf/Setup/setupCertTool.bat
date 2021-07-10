@echo off

echo ------------------------------------------------------------------
echo   Running SetupCertTool
echo ------------------------------------------------------------------
echo.

REM Build the tool
ECHO Building FindPrivateKey
msbuild /v:m FindPrivateKey\CS\FindPrivateKey.csproj
If ERRORLEVEL 1 goto error01

ECHO Copying FindPrivateKey to %ProgramFiles%\ServiceModelSampleTools
mkdir "%ProgramFiles%\ServiceModelSampleTools"
copy FindPrivateKey\CS\bin\FindPrivateKey.exe "%ProgramFiles%\ServiceModelSampleTools"
If ERRORLEVEL 1 goto error01

ECHO Finished.  Run CleanupCertTool.bat to remove this tool.

echo.
echo ------------------------------------------------------------------------------
echo   Setup completed successfully. Run CleanupCertTool.bat to remove this tool.
echo ------------------------------------------------------------------------------
goto :eof

:error01
echo -----------------------
echo   Error Running Setup
echo -----------------------
echo Try executing this script in a Visual Studio command prompt (running as  
echo administrator) if you have not already.
goto :eof

