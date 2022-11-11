rem ----------------------------------------------------------------------------------------------
rem  GTP STRATUS 2022 install script for DSI Revit ToolKit 
rem  This batch file gets bundled with the installer for DSI Revit Toolkit 
rem  This script is meant to be included in a self-extracting EXE, and run after decompression
rem         You can run:
rem                          install -dev
rem         And it will use the development machine build path as the source of the
rem         revit manifest files.
rem ----------------------------------------------------------------------------------------------
  
  set UnzipPath=%cd%
  echo Current Directory = %cd%
  set ZipTop=DSIRevitToolkit
  set DSIRoot=%LOCALAPPDATA%\dsi-revit-toolkit
  set DEV=0
  if /I (%1)==(-dev) set DSIRoot=C:\repos\DSI\revit-toolkit\src\bin
  if /I (%1)==(-dev) set DEV=1
  mkdir "%DSIRoot%\" 2>nul
  call :RevitYear 2018
  call :RevitYear 2019
  call :RevitYear 2020
  call :RevitYear 2021
  call :RevitYear 2022
  call :RevitYear 2023
goto :EOF


rem ----------------------------------------------------------------------------------------------
:RevitYear
  rem Cleanup old manifests
  del "C:\ProgramData\Autodesk\Revit\Addins\%1\DSIToolKit - %1.addin" 1>nul 2>nul
  del "C:\ProgramData\Autodesk\Revit\Addins\%1\DSIToolKit - Debug %1.addin" 1>nul 2>nul
  
  rem We copy all the DSI Revit Addin DLLs to the DSIRoot
  set instdir=%DSIRoot%\%1
  if (%DEV%)==(0) mkdir "%instdir%\" 2>nul  
  if (%DEV%)==(0) xcopy .\%ZipTop%\%1\*.* "%instdir%\" /s /q /y 1>nul 2>nul
  
  rem create Manifest
  set manifest=C:\ProgramData\Autodesk\Revit\Addins\%1\DSIRevitToolkit%1.addin
  echo ^<?xml version="1.0" encoding="utf-8" ?^> > %manifest%
  echo ^<RevitAddIns^> >> %manifest%
  echo   ^<AddIn Type="Application"^> >> %manifest%
  echo     ^<Name^>DSI Toolkit^</Name^> >> %manifest%
  echo     ^<Description^>DSI Toolkit Ribbon for Revit %1^</Description^> >> %manifest%
  echo     ^<Assembly^>%instdir%\Debug\DSIRevitToolkit.dll^</Assembly^> >> %manifest%
  echo     ^<FullClassName^>DSI.Application^</FullClassName^> >> %manifest%
  echo     ^<ClientId^>2ca60eee-f672-47f5-bb60-8764fa0bfe33^</ClientId^> >> %manifest%
  echo     ^<VendorId^>us.dsi^</VendorId^> >> %manifest%
  echo     ^<VendorDescription^>Dynamic Systems, Inc.^</VendorDescription^> >> %manifest%
  echo   ^</AddIn^> >> %manifest%
  echo ^</RevitAddIns^> >> %manifest%
goto :EOF

