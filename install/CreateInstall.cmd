@echo off
rem -------------------------------------------------------------------
rem     This batch script will take your source code 
rem     delete the build, and rebuild it. It will then
rem     create an EXE file that is an installer.
rem
rem     This program uses the paid version of WinZip
rem     that turns ZIPs into EXE files.
rem
rem     ----------------------------------------------------
rem     Options:
rem     ----------------------------------------------------
rem     -D:   To Create a Debug version of the installer, use:
rem             CreateInstall -D
rem          By default, creates a RELEASE version of the DLLS.
rem
rem     -S:  To create a ZIP copy of the source code:
rem             CreateInstall -S
rem          This would be for the case that you are at GTP,
rem          and working on the code for DSI, and want to send
rem          back a copy of all your changes to DSI.
rem
rem     -R:  To run the install after it is created
rem             CreateInstall -R
rem             Warning: this replaces your manifests files
rem
rem     Written for DSI, for their Revit Toolkit
rem     8 Nov 2022
rem     Steve.Rives@gogtp.com
rem     GTP Services
rem
rem -------------------------------------------------------------------

:ENV_VARS
    rem Change these if you move the directory where this code lives
    rem Source Path is the place under which there is the dsi-toolkit.sln file:
	set SOURCEPATH=C:\repos\DSI\revit-toolkit
	set SOURCE_ZIP=C:\repos\DSI\dsi-revit-toolkit-source.zip
	set Text=%SOURCEPATH%\install\Install.message.txt
	set About=%SOURCEPATH%\install\Install.message.txt

    rem We will STAGE the files for the installer to here:
	set ZipTop=DSIRevitToolkit
	set ZipFile=c:\%ZipTop%\%ZipTop%.zip
	set EXEFile=c:\%ZipTop%\%ZipTop%.exe

    rem We use a tool that turns a ZIP into an EXE (it is installed here):
	set WZIP=C:\Program Files (x86)\WinZip Self-Extractor\WZIPSE32.EXE
	
	rem If you have a signing cert, point to it here (if not, don't worry about it):
	set pwFile=C:\repos\secrets\sign_pw.cmd
	set signTool=C:\Program Files (x86)\Windows Kits\10\App Certification Kit\signtool.exe
	set signFile=C:\repos\Stratus\Certificates\GTPServices.pfx
	if not exist "%signFile%" set signFile=C:\repos\Stratus\Certificates\GTPServices,LLC.pfx

rem -------------------------------------------------------------------
:COMMAND_LINE_OPTIONS
	set WHAT=Release
	set ZIP_SOURCE=0
	set RUN_INSTALL=0
	set LOOP=0
	:LOOP_TOP
      set /A LOOP=LOOP+1
	  if /I (%1)==(-D) set WHAT=Debug
	  if /I (%1)==(-D) shift
	  if /I (%1)==(-S) set ZIP_SOURCE=1
	  if /I (%1)==(-S) shift
	  if /I (%1)==(-R) set RUN_INSTALL=1
	  if /I (%1)==(-R) shift
	if not (%LOOP%) == (4) goto :LOOP_TOP

rem -------------------------------------------------------------------
:CLEAN
echo CLEAN bin directory "%SOURCEPATH%\src\bin\"
rd /s /q "%SOURCEPATH%\src\bin\" 1>nul 2>nul
rd /s /q "%SOURCEPATH%\src\obj\" 1>nul 2>nul

rem -------------------------------------------------------------------
:ZIPSOURCE
if (%ZIP_SOURCE%)==(0) goto :BUILD
echo Creating Source CODE Zip at %SOURCE_ZIP%
del "%SOURCE_ZIP%" 1>nul 2>nul
if exist "%SOURCE_ZIP%" echo You have %SOURCE_ZIP% open. Close it.
powershell Compress-Archive "%SOURCEPATH%" -DestinationPath "%SOURCE_ZIP%"

rem -------------------------------------------------------------------
:BUILD
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


rem -------------------------------------------------------------------
:READY
echo Ready to create DSI Revit Install at c:\%ZipTop%\
c:
echo Signing all relevant EXE files
call sign.cmd

echo Create staging area at c:\%ZipTop%\ where we will build our ZIP and EXE
mkdir "c:\%ZipTop%\%ZipTop%\" 2>nul
mkdir "c:\%ZipTop%\%ZipTop%\2018" 2>nul
mkdir "c:\%ZipTop%\%ZipTop%\2019" 2>nul
mkdir "c:\%ZipTop%\%ZipTop%\2020" 2>nul
mkdir "c:\%ZipTop%\%ZipTop%\2021" 2>nul
mkdir "c:\%ZipTop%\%ZipTop%\2022" 2>nul
mkdir "c:\%ZipTop%\%ZipTop%\2023" 2>nul

echo Erase previous version of the staged files
del "c:\%ZipTop%\%ZipTop%\*.*" /s /q 1>nul 2>nul
del "c:\%ZipTop%\%ZipTop%\2018" /s /q 1>nul 2>nul
del "c:\%ZipTop%\%ZipTop%\2019" /s /q 1>nul 2>nul
del "c:\%ZipTop%\%ZipTop%\2020" /s /q 1>nul 2>nul
del "c:\%ZipTop%\%ZipTop%\2021" /s /q 1>nul 2>nul
del "c:\%ZipTop%\%ZipTop%\2022" /s /q 1>nul 2>nul
del "c:\%ZipTop%\%ZipTop%\2023" /s /q 1>nul 2>nul


rem -------------------------------------------------------------------
:STAGE
echo Copying all %WHAT% files to ZIP dir for compression
copy "%SOURCEPATH%\install\install.bat" "c:\%ZipTop%\%ZipTop%\"

rem ------------ Check for files NOT to copy to the ZIP ---------------
set EXCLUDE=
if exist "%SOURCEPATH%\install\excludeFiles.txt" echo Excluding the following files from staging:
if exist "%SOURCEPATH%\install\excludeFiles.txt" type "%SOURCEPATH%\install\excludeFiles.txt"
if exist "%SOURCEPATH%\install\excludeFiles.txt" echo.
if exist "%SOURCEPATH%\install\excludeFiles.txt" set EXCLUDE=/EXCLUDE:excludeFiles.txt

echo Current Directory: %cd%
echo Stage Revit 2018 %WHAT% addin: c:\%ZipTop%\%ZipTop%\2018\%WHAT%\
echo xcopy "%SOURCEPATH%\src\bin\2018\%WHAT%\*.*" "c:\%ZipTop%\%ZipTop%\2018\%WHAT%\" %EXCLUDE% /S /Q /Y
xcopy "%SOURCEPATH%\src\bin\2018\%WHAT%\*.*" "c:\%ZipTop%\%ZipTop%\2018\%WHAT%\" %EXCLUDE% /S /Q /Y  2>nul

echo Stage Revit 2019 %WHAT% addin: c:\%ZipTop%\%ZipTop%\2019\%WHAT%\
echo xcopy "%SOURCEPATH%\src\bin\2019\%WHAT%\*.*" "c:\%ZipTop%\%ZipTop%\2019\%WHAT%\" %EXCLUDE% /S /Q /Y 
xcopy "%SOURCEPATH%\src\bin\2019\%WHAT%\*.*" "c:\%ZipTop%\%ZipTop%\2019\%WHAT%\" %EXCLUDE% /S /Q /Y  2>nul

echo Stage Revit 2020 %WHAT% addin: c:\%ZipTop%\%ZipTop%\2020\%WHAT%\
echo xcopy "%SOURCEPATH%\src\bin\2020\%WHAT%\*.*" "c:\%ZipTop%\%ZipTop%\2020\%WHAT%\" %EXCLUDE% /S /Q /Y
xcopy "%SOURCEPATH%\src\bin\2020\%WHAT%\*.*" "c:\%ZipTop%\%ZipTop%\2020\%WHAT%\" %EXCLUDE% /S /Q /Y  2>nul

echo Stage Revit 2021 %WHAT% addin: c:\%ZipTop%\%ZipTop%\2021\%WHAT%\
echo xcopy "%SOURCEPATH%\src\bin\2021\%WHAT%\*.*" "c:\%ZipTop%\%ZipTop%\2021\%WHAT%\" %EXCLUDE% /S /Q /Y
xcopy "%SOURCEPATH%\src\bin\2021\%WHAT%\*.*" "c:\%ZipTop%\%ZipTop%\2021\%WHAT%\" %EXCLUDE% /S /Q /Y  2>nul

echo Stage Revit 2022 %WHAT% addin: c:\%ZipTop%\%ZipTop%\2022\%WHAT%\
echo xcopy "%SOURCEPATH%\src\bin\2022\%WHAT%\*.*" "c:\%ZipTop%\%ZipTop%\2022\%WHAT%\" %EXCLUDE% /S /Q /Y
xcopy "%SOURCEPATH%\src\bin\2022\%WHAT%\*.*" "c:\%ZipTop%\%ZipTop%\2022\%WHAT%\" %EXCLUDE% /S /Q /Y  2>nul

echo Stage Revit 2023 %WHAT% addin: c:\%ZipTop%\%ZipTop%\2023\%WHAT%\
echo xcopy "%SOURCEPATH%\src\bin\2023\%WHAT%\*.*" "c:\%ZipTop%\%ZipTop%\2023\%WHAT%\" %EXCLUDE% /S /Q /Y
xcopy "%SOURCEPATH%\src\bin\2023\%WHAT%\*.*" "c:\%ZipTop%\%ZipTop%\2023\%WHAT%\" %EXCLUDE% /S /Q /Y  2>nul

echo Files ready to be turned into a Zip


rem -------------------------------------------------------------------
:ZIP
echo Creating Zip
rem Delete the oldcopu
del "c:\%ZipTop%\%ZipTop%.zip" 1>nul 2>nul
if exist "c:\%ZipTop%\%ZipTop%.zip" echo ERROR: you have "c:\%ZipTop%\%ZipTop%.zip" open. Close it.
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

echo Running WinZip Self Extraction Creation tool (consider getting licensed version to avoid unwanted annoy messages)
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
if (%RUN_INSTALL%)==(0) goto :FINAL_NOTE
echo.
echo --------------------- Running Install Program --------------------
echo          Running: c:\%ZipTop%\%ZipTop%
echo Warning: this replaces your manifests files
echo          Run install.bat -dev to reset your manifests for dev testing
"c:\%ZipTop%\%ZipTop%"
echo          Ran the latestet instance of the DSI toolkit installer.

rem -------------------------------------------------------------------
:FINAL_NOTE
echo.
if (%ZIP_SOURCE%)==(1) echo Your SOURCE CODE is zipped here: %SOURCE_ZIP% (send this to DSI)
if (%ZIP_SOURCE%)==(0) echo If you want to create a ZIP file of the source code, re-run as: CreateInstall -S
echo Your INSTALLER is: c:\%ZipTop%\%ZipTop%.EXE
echo.
