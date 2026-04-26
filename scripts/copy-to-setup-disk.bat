@echo off

:loop

cd /d %~dp0

set /p target=Enter targeted location:

if not exist "%target%" (
  echo %target% does not exist.
  echo.
  goto loop
)

xcopy downloads %target% /r

goto loop