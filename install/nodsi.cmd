@echo off
rem -------------------------------------------------------------------
rem
rem Uninstall the DSI toolkit from all versions of Revit
rem This is useful during testing
rem
rem     8 Nov 2022
rem     Steve.Rives@gogtp.com
rem     GTP Services
rem
rem -------------------------------------------------------------------

  set DSIRoot=%LOCALAPPDATA%\dsi-revit-toolkit
  set WHAT=Release
  set ZipTop=DSIRevitToolkit

  set adpath=C:\ProgramData\Autodesk\Revit\Addins\
  if exist "%adpath%" goto :ready

  set adpath=C:\%USERNAME%\AppData\Roaming\Autodesk\Revit\Addins\
  if exist "%adpath%" goto :ready
  
  set adpath=C:\%USERNAME%\AppData\Roaming\Autodesk\ApplicationPlugins\
  if exist "%adpath%" goto :ready
  
  set adpath=C:\ProgramData\Autodesk\Revit\Addins\
  echo ERROR: Could not find Autodesk path: %adpath%, we will just use the most common one 
  
:ready

  echo Delete Build files (if you want them back, just run build.cmd)
  rd "..\src\bin\" /s /q 1>nul 2>nul
  rd "..\src\obj\" /s /q 1>nul 2>nul
  echo Delete Staging files (if you want them back, just run CreateInstall.cmd)
  rd "c:\%ZipTop%\%ZipTop%\" /s /q 1>nul 2>nul
  
  echo Autodesk Revit Addin Location: %adpath%
  
  call :RevitYear 2018
  call :RevitYear 2019
  call :RevitYear 2020
  call :RevitYear 2021
  call :RevitYear 2022
  call :RevitYear 2023
  
  goto :EOF


:RevitYear
    echo.
    set instdir=%DSIRoot%\%1

    if exist "%instdir%\" echo Delete installed binaries for %1 (%instdir%\)	
	rd "%instdir%\" /s /q 1>nul 2>nul
	
	echo Delete installed binaries for %1 DEBUG (%instdir%\)
	
	set manifest=%adpath%\%1\DSIRevitToolkit%1.addin
	if not exist "%manifest%" echo DSI Revit %1 Toolkit Addin Manifest NOT FOUND
	if exist "%manifest%"     echo DSI Revit %1 Toolkit Addin Manifest DELETED
	if exist "%manifest%" del %manifest% 1>nul 2>nul
	
goto :EOF