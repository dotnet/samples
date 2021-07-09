@echo off
setlocal

if /I "" == "%1"  (
    set __ETWTracingSampleFilename="c:\logs\ETWTracingSampleLog.etl"
    goto :NoParams
)

if /I "/?" == "%1" (
    goto :Usage
)

if /I "-?" == "%1" (
    goto :Usage
)

set __ETWTracingSampleFilename="%1"


:NoParams
@echo Cleaning up old session...
call CleanupETW.bat
@echo done.
@echo.

@echo Setting up new session
call logman create trace ETWTracingSampleSession -o %__ETWTracingSampleFilename% -p "{411a0819-c24b-428c-83e2-26b41091702e}"
call logman start ETWTracingSampleSession
@echo done.
goto :EOF


:Usage
@echo Usage: SetupETW.bat [filename] [/?]
@echo.
@echo Parameters:
@echo.
@echo filename      : The path of the .etl output file passed to logman. Default: c:\logs\ETWTracingSampleLog.etl
@echo ?             : Display this help
@echo.
@echo Example:
@echo    SetupETW.bat c:\logs\ETWTracingSampleLog.etl

endlocal
