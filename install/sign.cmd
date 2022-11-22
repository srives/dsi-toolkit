@echo off

set signFile=C:\repos\Stratus\Certificates\GTPServices.pfx
rem We need a password for the cert. We get it from this sign_pw.cmd, but however you get it, set pw=password
set pwFile=C:\repos\secrets\sign_pw.cmd

rem -------------------------------------------------------
rem   Sign the DLLs built for DSI Toolkit
rem   This is not essential, but signed files are easier
rem   to distribute
rem
rem     8 Nov 2022
rem     Steve.Rives@gogtp.com
rem     GTP Services
rem
rem -------------------------------------------------------


if not exist "%signFile%" set signFile=C:\repos\Stratus\Certificates\GTPServices,LLC.pfx
set signTool=C:\Program Files (x86)\Windows Kits\10\App Certification Kit\signtool.exe

rem Source Path is the place under which there is the dsi-toolkit.sln file
if "%SOURCEPATH%"=="" set SOURCEPATH=C:\repos\DSI\revit-toolkit


if not exist "%signTool%" echo Not Signing DLLs, missing the signtool.exe file that is in the Windows Kits.
if not exist "%pwFile%" echo Not Signing DLLs, missing password file %pwFile%, which should have a on line "set pw=" command in it.
if not exist "%signFile%" echo Not Signing DLLs, missing PFX signature file.
if not exist "%signFile%" goto :EOF
if not exist "%pwFile%" goto :EOF
if not exist "%signTool%" goto :EOF
call %pwFile%

echo Sign all EXEs we place on the client computer
echo About to sign DSI Revit Toolkit

call :sign_it 2018
call :sign_it 2019
call :sign_it 2020
call :sign_it 2021
call :sign_it 2022
call :sign_it 2023

goto :EOF

rem ----------------------------------------------------------------------------------------------
:sign_it
rem SIGN Debug EXE 
if not exist "%SOURCEPATH%\src\bin\%1\Debug\DSIRevitToolkit.dll" goto :RELEASE
echo SIGN Debug DLL for %1
"%signTool%" sign /td SHA256 /fd SHA256 /f %signFile% /p %pw% /tr http://timestamp.digicert.com/ "%SOURCEPATH%\src\bin\%1\Debug\DSIRevitToolkit.dll"

:RELEASE
if not exist "%SOURCEPATH%\src\bin\%1\Release\DSIRevitToolkit.dll" goto :EOF
echo SIGN Release DLL for %1
"%signTool%" sign /td SHA256 /fd SHA256 /f %signFile% /p %pw% /tr http://timestamp.digicert.com/ "%SOURCEPATH%\src\bin\%1\Release\DSIRevitToolkit.dll"

goto :EOF