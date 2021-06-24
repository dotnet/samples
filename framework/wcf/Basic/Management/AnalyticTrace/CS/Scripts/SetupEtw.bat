@echo off
setlocal


if /I "" == "%1" (
    set __EtwAnalyticTracingFilename="c:\logs\EtwAnalyticTracingLog.etl"
    goto :NoParams
)

if /I "/?" == "%1" (
    goto :Usage
)

if /I "-?" == "%1" (
    goto :Usage
)


set __EtwAnalyticTracingFilename="%1"

:NoParams
@echo Cleaning up old session...
call CleanupEtw.bat
@echo done.
@echo.

@echo Setting up new session
@echo The output file is %__EtwAnalyticTracingFilename%
call logman create trace EtwAnalyticTracingSession -ow -o %__EtwAnalyticTracingFilename% -p "{c651f5f6-1c0d-492e-8ae1-b4efd7c9d503}" -ets

@echo done.
goto :EOF


:Usage
@echo Usage: SetupEtw.bat [filename] [/?]
@echo.
@echo Parameters:
@echo.
@echo filename      : The path of the .etl output file passed to logman. Default: c:\logs\EtwAnalyticTracingLog.etl
@echo ?             : Display this help
@echo.
@echo Example:
@echo    SetupEtw.bat c:\logs\EtwAnalyticTracingLog.etl

endlocal