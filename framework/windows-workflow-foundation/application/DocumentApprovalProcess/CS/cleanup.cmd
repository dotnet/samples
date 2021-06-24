@echo off

echo --------------------------------------------------------------------------
echo   Cleaning up changes made by the Document Approval Process sample setup
echo --------------------------------------------------------------------------
echo.

echo Removing sample databases ...
sqlcmd -S localhost\SQLExpress -E -i "SetupFiles\cleanup.sql" -o cleanup-sqlcmd.log
If ERRORLEVEL 1 goto sqlerror

del cleanup-sqlcmd.log

echo.
echo ----------------------------------
echo   Cleanup completed successfully
echo ----------------------------------

goto :eof

:sqlerror
echo -------------------------
echo   Error Running Cleanup
echo -------------------------
echo A sql command in the cleanup script failed to execute.  This is most likely
echo because your current user account does not have the required access to the
echo sample database or because the server name used in the script 
echo ('localhost\SQLExpress') does not match your SQL server name.  See 
echo cleanup-sqlcmd.log for the output from the failed sqlcmd command.
goto :eof
