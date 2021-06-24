@echo off

logman query ETWTracingSampleSession 2>1 >NUL

IF %ERRORLEVEL% EQU 0 (
    logman stop -ets ETWTracingSampleSession
    logman delete ETWTracingSampleSession
) ELSE (
    @echo Nothing to cleanup
)
