@echo off
rem -------------------------------------------------------------------
rem     This batch script will take your source code output
rem     and create an EXE file that is an installer.
rem     This program uses the paid version of WinZip
rem     that turns ZIPs into EXE files.
rem     Written for DSI, for their Revit Toolkit
rem
rem     Debug vs. Release
rem
rem       By default,
rem             Creates RELEASE version of the installer
rem       To Create DeBUG version, use:
rem             CreateInstall -D
rem
rem     8 Nov 2022
rem     Steve.Rives@gogtp.com
rem     GTP Services
rem
rem -------------------------------------------------------------------

rem ------------------------- Configuration ---------------------------
rem Change these if you move the code path
rem Source Path is the place under which there is the dsi-toolkit.sln file.
	set SOURCEPATH=C:\repos\DSI\revit-toolkit
	set pwFile=C:\repos\secrets\sign_pw.cmd
	set signTool=C:\Program Files (x86)\Windows Kits\10\App Certification Kit\signtool.exe
	set WHAT=Release
	set ZipTop=DSIRevitToolkit
	set WZIP=C:\Program Files (x86)\WinZip Self-Extractor\WZIPSE32.EXE
	set ZipFile=c:\%ZipTop%\%ZipTop%.zip
	set EXEFile=c:\%ZipTop%\%ZipTop%.exe
	set Text=%SOURCEPATH%\install\Install.message.txt
	set About=%SOURCEPATH%\install\Install.message.txt
	if /I (%1)==(-D) set WHAT=Debug
	set signFile=C:\repos\Stratus\Certificates\GTPServices.pfx
	if not exist "%signFile%" set signFile=C:\repos\Stratus\Certificates\GTPServices,LLC.pfx

rem -------------------------------------------------------------------
echo Building DSI Revit Toolkit
echo Creating %WHAT% version of the installer (pass in -D to this script to create the DEBUG version)
call .\build.cmd %WHAT%
  set found=0
  call :CHECK 2018
  call :CHECK 2019
  call :CHECK 2020
  call :CHECK 2021
  call :CHECK 2022
  call :CHECK 2023
  
  if (%found%)==(0) echo No files found to create install package. Missing DLL DSIRevitToolkit.dll for all versions of Revit
  if (%found%)==(0) goto :EOF  
goto :READY

rem -------------------------------------------------------------------
rem -- Subroutine to CHECK to make sure we have something to install --
	:CHECK
	  if not exist "%SOURCEPATH%\src\bin\%1\%WHAT%\DSIRevitToolkit.dll"   echo %1 %WHAT%  DSI Revit Toolkit is Missing (DSIRevitToolkit.dll)
	  if exist "%SOURCEPATH%\src\bin\%1\%WHAT%\DSIRevitToolkit.dll"       set found=1
	  if exist "%SOURCEPATH%\src\bin\%1\%WHAT%\DSIRevitToolkit.dll"       echo Including %1 %WHAT%  DSI Revit Toolkit in Installer
	goto :EOF
rem -------------------------------------------------------------------


:READY
rem -------------------------------------------------------------------
echo Ready to create DSI Revit Install at c:\%ZipTop%\
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


rem -------------------------------------------------------------------
echo Copying all %WHAT% files to ZIP dir for compression
copy "%SOURCEPATH%\install\install.bat" "c:\%ZipTop%\%ZipTop%\"
echo Stage Revit 2018 %WHAT% addin
xcopy "%SOURCEPATH%\src\bin\2018\%WHAT%\*.*" "c:\%ZipTop%\%ZipTop%\2018\%WHAT%\" /S /Q /Y  2>nul
echo Stage Revit 2019 %WHAT% addin
xcopy "%SOURCEPATH%\src\bin\2019\%WHAT%\*.*" "c:\%ZipTop%\%ZipTop%\2019\%WHAT%\" /S /Q /Y  2>nul
echo Stage Revit 2020 %WHAT% addin
xcopy "%SOURCEPATH%\src\bin\2020\%WHAT%\*.*" "c:\%ZipTop%\%ZipTop%\2020\%WHAT%\" /S /Q /Y  2>nul
echo Stage Revit 2021 %WHAT% addin
xcopy "%SOURCEPATH%\src\bin\2021\%WHAT%\*.*" "c:\%ZipTop%\%ZipTop%\2021\%WHAT%\" /S /Q /Y  2>nul
echo Stage Revit 2022 %WHAT% addin
xcopy "%SOURCEPATH%\src\bin\2022\%WHAT%\*.*" "c:\%ZipTop%\%ZipTop%\2022\%WHAT%\" /S /Q /Y  2>nul
echo Stage Revit 2023 %WHAT% addin
xcopy "%SOURCEPATH%\src\bin\2023\%WHAT%\*.*" "c:\%ZipTop%\%ZipTop%\2023\%WHAT%\" /S /Q /Y  2>nul


rem -------------------------------------------------------------------
echo Files ready to be turned into a Zip
if not exist "c:\%ZipTop%\%ZipTop%.zip" goto :ZIP
del "c:\%ZipTop%\%ZipTop%.zip"


rem -------------------------------------------------------------------
:ZIP
echo Creating Zip
powershell Compress-Archive "c:\%ZipTop%\%ZipTop%" "c:\%ZipTop%\%ZipTop%.zip"
if not exist "c:\%ZipTop%\%ZipTop%.exe" goto :EXE
mkdir "c:\%ZipTop%\previous\" 2>nul
echo Backing up old installer
copy "c:\%ZipTop%\%ZipTop%.exe" "c:\%ZipTop%\previous\%ZipTop%.%RANDOM%.exe" 1>nul

rem -------------------------------------------------------------------
:EXE
echo Creating Self-Extracting EXE
if not exist "%WZIP%" echo Cannot create self-extracting EXE (missing %WZIP%)
if not exist "%WZIP%" echo Your install package is in a ZIP called, c:\%ZipTop%\%ZipTop%.zip
if not exist "%WZIP%" echo To install, unzip that file and run install.bat
if not exist "%WZIP%" goto :EOF

echo Running WinZip Self Extraction Creation tool (consider getting licensed version to avoid unwanted )
"%WZIP%" %ZipFile% -auto -setup -t %Text% -a %About% -c .\%ZipTop%\install.bat

if not exist "c:\%ZipTop%\%ZipTop%.EXE" echo Your self extracting EXE failed get created.
if not exist "c:\%ZipTop%\%ZipTop%.EXE" echo The following command failed:
if not exist "c:\%ZipTop%\%ZipTop%.EXE" echo "%WZIP%" %ZipFile% -auto -setup -t %Text% -a %About% -c .\%ZipTop%\install.bat
if not exist "c:\%ZipTop%\%ZipTop%.EXE" goto :EOF

rem -------------------------------------------------------------------
:SIGN
echo Signing Installer EXE file (this is not required, if it doesn't get signed, the installer will work still)
if not exist "%signTool%" echo Not Signing Installer, missing the signtool.exe file that is in the Windows Kits.
if not exist "%pwFile%" echo Not Signing Installer, missing password file %pwFile%, which should have a on line "set pw=" command in it.
if not exist "%signFile%" echo Not Signing Installer, missing PFX signature file.
if not exist "%signFile%" goto :RUN
if not exist "%pwFile%" goto :RUN
if not exist "%signTool%" goto :RUN
call %pwFile%

rem Sign the self-extracting EXE file
"%signTool%" sign /td SHA256 /fd SHA256 /f "%signFile%" /p %pw% /tr http://timestamp.digicert.com/ %EXEFile%


rem -------------------------------------------------------------------
:RUN
echo.
echo --------------------- Running Install Program --------------------
echo          Running: c:\%ZipTop%\%ZipTop%
echo Warning: this replaces your manifests files
echo          Run install.bat -dev to reset your manifests for dev testing

"c:\%ZipTop%\%ZipTop%"
