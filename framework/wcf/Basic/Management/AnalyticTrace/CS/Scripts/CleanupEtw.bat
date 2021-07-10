@echo off

logman query ETWAnalyticTracingSession -ets 2>1 >NUL

IF %ERRORLEVEL% EQU 0 (
    logman stop -ets ETWAnalyticTracingSession
) ELSE (
    @echo Nothing to cleanup
)
