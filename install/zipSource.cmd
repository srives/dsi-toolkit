@echo off
rem -------------------------------------------------------------------
rem
rem     Written for DSI, for their Revit Toolkit
rem     This will delete the binaries, then ZIP up the source code
rem     so that you can send it to DSI so they can archive it.
rem
rem     6 Dev 2022
rem     Steve.Rives@gogtp.com
rem     GTP Services
rem
rem -------------------------------------------------------------------

cd..	
set SOURCEPATH=%cd%
set SOURCE_ZIP=%SOURCEPATH%-source-%TODAY%.zip
rem TODAY=Year-Month-Day
for /F "tokens=1-5 delims=/ " %%i in ('date /t') do set TODAY=%%l-%%j-%%k

cd install
call nodsi.cmd

del "%SOURCE_ZIP%" 1>nul 2>nul
powershell Compress-Archive "%SOURCEPATH%" -DestinationPath "%SOURCE_ZIP%"

echo.
echo Your Sourcecode is zipped here: "%SOURCE_ZIP%"
echo Upload to DSI Sharepoint here:
echo     https://gogtp.sharepoint.com/:f:/r/sites/CustomerSharedFiles/Shared%%20Documents/GTP%%20STRATUS/File%%20Share%%20to%%20Customers/DSI?csf=1^&web=1^&e=ygayfZ
echo.

