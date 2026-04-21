@echo off

:loop

cd /d %~dp0

set /p name=Enter project name:

if not exist "source-codes\%name%" (
echo Project %name% does not exist.
echo.
goto loop
)

cd /d "source-codes\%name%"

dotnet run

goto loop