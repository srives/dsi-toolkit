@echo off
set ZipTop=DSIRevitToolkit
set WHAT=Release
set WHAT=Debug

rem ----------------------------------------------------------
echo This will copy DSI Revit Install to c:\%ZipTop%\
echo Build Release Mode Switch

rem ----------------------------------------------------------
set ZipFile=c:\%ZipTop%\%ZipTop%.zip
set EXEFile=c:\%ZipTop%\%ZipTop%.exe
set Text=C:\repos\DSI\revit-toolkit\install\Install.message.txt
set About=C:\repos\DSI\revit-toolkit\install\Install.message.txt

rem ----------------------------------------------------------
c:
echo Signing all relevant EXE files
call sign.cmd

echo Create staging area at c:\%ZipTop%\ where we will build our ZIP and EXE
mkdir c:\%ZipTop%\%ZipTop%\ 2>nul
mkdir c:\%ZipTop%\%ZipTop%\2018 2>nul
mkdir c:\%ZipTop%\%ZipTop%\2019 2>nul
mkdir c:\%ZipTop%\%ZipTop%\2020 2>nul
mkdir c:\%ZipTop%\%ZipTop%\2021 2>nul
mkdir c:\%ZipTop%\%ZipTop%\2022 2>nul
mkdir c:\%ZipTop%\%ZipTop%\2023 2>nul


rem ----------------------------------------------------------
echo Copying to ZIP dir for compression
copy C:\repos\DSI\revit-toolkit\install\install.bat c:\%ZipTop%\%ZipTop%\
xcopy C:\repos\DSI\revit-toolkit\src\bin\2018\%WHAT%\*.* c:\%ZipTop%\%ZipTop%\2018\%WHAT%\ /S /Q /Y  2>nul
xcopy C:\repos\DSI\revit-toolkit\src\bin\2019\%WHAT%\*.* c:\%ZipTop%\%ZipTop%\2019\%WHAT%\ /S /Q /Y  2>nul
xcopy C:\repos\DSI\revit-toolkit\src\bin\2020\%WHAT%\*.* c:\%ZipTop%\%ZipTop%\2020\%WHAT%\ /S /Q /Y  2>nul
xcopy C:\repos\DSI\revit-toolkit\src\bin\2021\%WHAT%\*.* c:\%ZipTop%\%ZipTop%\2021\%WHAT%\ /S /Q /Y  2>nul
xcopy C:\repos\DSI\revit-toolkit\src\bin\2022\%WHAT%\*.* c:\%ZipTop%\%ZipTop%\2022\%WHAT%\ /S /Q /Y  2>nul
xcopy C:\repos\DSI\revit-toolkit\src\bin\2023\%WHAT%\*.* c:\%ZipTop%\%ZipTop%\2023\%WHAT%\ /S /Q /Y  2>nul


rem ----------------------------------------------------------
echo Files ready to be turned into a Zip
if not exist c:\%ZipTop%\%ZipTop%.zip goto :ZIP
del c:\%ZipTop%\%ZipTop%.zip


rem ----------------------------------------------------------
:ZIP
echo Creating Zip
powershell Compress-Archive c:\%ZipTop%\%ZipTop% c:\%ZipTop%\%ZipTop%.zip
if not exist c:\%ZipTop%\%ZipTop%.exe goto :EXE
mkdir c:\%ZipTop%\previous\  2>nul
echo Backing up old installer
copy c:\%ZipTop%\%ZipTop%.exe c:\%ZipTop%\previous\%ZipTop%.%RANDOM%.exe


rem ----------------------------------------------------------
:EXE
echo Creating Self-Extracting EXE
"C:\Program Files (x86)\WinZip Self-Extractor\WZIPSE32.EXE" %ZipFile% -auto -setup -t %Text% -a %About% -c .\%ZipTop%\install.bat
"C:\Program Files (x86)\Windows Kits\10\App Certification Kit\signtool.exe" sign /td SHA256 /fd SHA256 /f "c:\repos\STRATUS\Certificates\GTPServices,LLC.pfx" /p Larkspur17 /tr http://timestamp.digicert.com/ %EXEFile%


echo.
echo   ----------------------------- Running Install Program -------------------------
echo                           Running: c:\%ZipTop%\%ZipTop%
echo                                 (look for Window)
c:\%ZipTop%\%ZipTop%
