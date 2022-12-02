@echo off
rem ----------------------------------------------------------------------------------------------
rem  GTP STRATUS 2022 install script for DSI Revit ToolKit 
rem  This batch file gets bundled with the installer for DSI Revit Toolkit 
rem  This script is meant to be included in a self-extracting EXE, and run after decompression
rem
rem  Developer Notes:
rem
rem         You can run:
rem                          install -dev
rem         And it will use the development machine build path as the source of the
rem         revit manifest files (a developer would do this while building/debugging.
rem
rem  To check install results, check this file:
rem
rem         %LOCALAPPDATA%\dsi-revit-toolkit\install.txt
rem
rem  8 Nov 2022
rem  Steve.Rives@gogtp.com
rem  GTP Services
rem
rem ----------------------------------------------------------------------------------------------
  
  set UnzipPath=%cd%
  echo Current Directory = %cd%
  set ZipTop=DSIRevitToolkit
  set DSIRoot=%ProgramFiles%\DSI\dsi-revit-toolkit
  set WHAT=Release
  set DEV=0
  if /I (%1)==(-dev) set DSIRoot=C:\repos\DSI\revit-toolkit\src\bin\x64
  if /I (%1)==(-dev) set DEV=1
  if /I (%1)==(-dev) set WHAT=Debug
  set installLog=%DSIRoot%\install.txt

  echo. >> "%installLog%"  
  echo. >> "%installLog%"
  echo ------------------------------------------------- >> "%installLog%"
  echo Running DSI Revit Toolkit Installer >> "%installLog%"
  date /T >> "%installLog%"
  time /T >> "%installLog%"

  echo Installer Contents: >> "%installLog%"
  echo ------------------------------------------------- >> "%installLog%"
  dir "%UnzipPath%" /s /b >> "%installLog%"
  echo ------------------------------------------------- >> "%installLog%"
  
  echo Putting DSI Toolkit files here: %DSIRoot% >> "%installLog%"
  mkdir "%DSIRoot%\" 2>nul
    
  rem Determine Autodesk Path, it can be one of three places  
  set adpath=C:\ProgramData\Autodesk\Revit\Addins\
  if exist "%adpath%" goto :ready

  set adpath=C:\%USERNAME%\AppData\Roaming\Autodesk\Revit\Addins\
  if exist "%adpath%" goto :ready
  
  set adpath=C:\%USERNAME%\AppData\Roaming\Autodesk\ApplicationPlugins\
  if exist "%adpath%" goto :ready
  
  set adpath=C:\ProgramData\Autodesk\Revit\Addins\
  echo ERROR: Could not find Autodesk path: %adpath%, we will just use the most common one
  echo ERROR: Could not find Autodesk path: %adpath%, we will just use the most common one >> "%installLog%"

  
:ready

  echo Autodesk Revit Addin Location: %adpath%
  echo Autodesk Revit Addin Location: %adpath% >> "%installLog%"

  call :RevitYear 2018
  call :RevitYear 2019
  call :RevitYear 2020
  call :RevitYear 2021
  call :RevitYear 2022
  call :RevitYear 2023

  echo DSI Revit Toolkit Install finished >> "%installLog%"
  echo Finished. Check install log: %installLog%  
  
goto :EOF

rem ----------------------------------------------------------------------------------------------
:RevitYear
rem For the passed in year, deposit the DSI related toolkit DLLs, and create a Revit manifest.
rem Copy DLLs from installer over to the user's appdata\local\dsi-revit-toolkit director
rem ----------------------------------------------------------------------------------------------

  echo ------------------- %1 ------------------ >> "%installLog%"
  
  rem If we are DEV=1, then we don't copy the DLLs over to any place (we run them out of the build dir)
  if (%DEV%)==(1) set DLL=%DSIRoot%\%WHAT% (Revit %1)\DSIRevitToolkit.dll
  if (%DEV%)==(1) echo Running Developer Mode
  if (%DEV%)==(1) echo Running Developer Mode >> "%installLog%"
  if (%DEV%)==(1) goto :make_manifest

  rem We copy all the DSI Revit Addin DLLs to the DSIRoot
  set instdir=%DSIRoot%\%1
  set DLL=%instdir%\%WHAT%\DSIRevitToolkit.dll
  echo Installing DSI Revit Toolkit for Revit %1
  echo Installing DSI Revit Toolkit for Revit %1 >> "%installLog%"

  rem Copy Files  
  mkdir "%instdir%\" 1>nul 2>nul  
  echo  xcopy "%UnzipPath%\%ZipTop%\%1\*.*" "%instdir%\" /s /q /y 
  echo  xcopy "%UnzipPath%\%ZipTop%\%1\*.*" "%instdir%\" /s /q /y >> "%installLog%"
  xcopy "%UnzipPath%\%ZipTop%\%1\*.*" "%instdir%\" /s /q /y >> "%installLog%"

  if not exist "%instdir%\%WHAT%\DSIRevitToolkit.dll" echo Cannot find "%instdir%\%WHAT%\DSIRevitToolkit.dll" >> "%installLog%"
  if not exist "%instdir%\%WHAT%\DSIRevitToolkit.dll" echo Cannot find "%instdir%\%WHAT%\DSIRevitToolkit.dll"
  if not exist "%instdir%\%WHAT%\DSIRevitToolkit.dll" echo xcopy "%UnzipPath%\%ZipTop%\%1\*.*" "%instdir%\" /s /q /y Failed
  rem if not exist "%instdir%\%WHAT%\DSIRevitToolkit.dll" if "%USERNAME%"=="xxxxxxxxxxxxxxxxxxxx" pause
  
  rem Depricated: Cleanup old manifests (probably not needed here, but just in case)
  del "C:\ProgramData\Autodesk\Revit\Addins\%1\DSIToolKit - %1.addin" 1>nul 2>nul
  del "C:\ProgramData\Autodesk\Revit\Addins\%1\DSIToolKit - Debug %1.addin" 1>nul 2>nul

  :make_manifest
  rem Create Manifest
  set manifest=%adpath%%1\DSIRevitToolkit%1.addin
  echo Creating %1 manifest, pointing to %DLL%
  echo Creating %1 manifest, pointing to %DLL% >> "%installLog%"
  
  echo ^<?xml version="1.0" encoding="utf-8" ?^> > "%manifest%"
  echo ^<RevitAddIns^> >> "%manifest%"
  echo   ^<AddIn Type="Application"^> >> "%manifest%"
  echo     ^<Name^>DSI Toolkit^</Name^> >> "%manifest%"
  echo     ^<Description^>DSI Toolkit Ribbon for Revit %1^</Description^> >> "%manifest%"
  echo     ^<Assembly^>%DLL%^</Assembly^> >> "%manifest%"
  echo     ^<FullClassName^>DSI.Application^</FullClassName^> >> "%manifest%"
  echo     ^<ClientId^>2ca60eee-f672-47f5-bb60-8764fa0bfe33^</ClientId^> >> "%manifest%"
  echo     ^<VendorId^>us.dsi^</VendorId^> >> "%manifest%"
  echo     ^<VendorDescription^>Dynamic Systems, Inc.^</VendorDescription^> >> "%manifest%"
  echo   ^</AddIn^> >> "%manifest%"
  echo ^</RevitAddIns^> >> "%manifest%"
  
  if exist "%manifest%" echo Created Manifest %manifest%.  >> "%installLog%"
  if not exist "%manifest%" echo Failed to created Manifest %manifest% (you do not have Revit %1 installed).
  if not exist "%manifest%" echo Failed to created Manifest %manifest% (you do not have Revit %1 installed).  >> "%installLog%"
  
goto :EOF