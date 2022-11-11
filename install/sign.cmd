@echo off

echo Sign all EXEs we place on the client computer
echo About to sign DSI Revit Toolkit

call :sign_it 2018
call :sign_it 2019
call :sign_it 2020
call :sign_it 2021
call :sign_it 2022
call :sign_it 2023

goto :EOF

:sign_it

rem echo ------------- SIGN Debug EXE -------------
if not exist C:\repos\DSI\revit-toolkit\src\bin\%1\Debug\DSIRevitToolkit.dll goto :RELEASE
"C:\Program Files (x86)\Windows Kits\10\App Certification Kit\signtool.exe" sign /td SHA256 /fd SHA256 /f "C:\repos\Stratus\Certificates\GTPServices.pfx" /p Larkspur17 /tr http://timestamp.digicert.com/ C:\repos\DSI\revit-toolkit\src\bin\%1\Debug\DSIRevitToolkit.dll

:RELEASE
if not exist C:\repos\DSI\revit-toolkit\src\bin\%1\Release\DSIRevitToolkit.dll goto :EOF
rem echo ------------- SIGN Release EXE -------------
"C:\Program Files (x86)\Windows Kits\10\App Certification Kit\signtool.exe" sign /td SHA256 /fd SHA256 /f "C:\repos\Stratus\Certificates\GTPServices.pfx" /p Larkspur17 /tr http://timestamp.digicert.com/ C:\repos\DSI\revit-toolkit\src\bin\%1\Release\DSIRevitToolkit.dll

goto :EOF