@echo off
rem ----------------------------------------------------------------------------------------------
rem  GTP STRATUS 2022 build script for DSI Revit ToolKit
rem  Builds x64.
rem         If you want to build ARM64, I don't think Revit supports that. 
rem
rem  Usage:
rem           build 2023
rem         Builds the Revit 2023 version of the DSI addin
rem
rem  8 Nov 2022
rem  Steve.Rives@gogtp.com
rem  GTP Services
rem
rem -----------------------------------------------------------------------


set VSVER=2022
set MSBUILD=C:\Program Files\Microsoft Visual Studio\2022\Professional\Msbuild\Current\Bin\MSBuild.exe
if not exist "%MSBUILD%" set VSVER=2019
if not exist "%MSBUILD%" set MSBUILD=C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe
if not exist "%MSBUILD%" set VSVER=2017
if not exist "%MSBUILD%" set MSBUILD=C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe
if not exist "%MSBUILD%" echo Could not find MSBuild.exe. Change your build.cmd script.
if not exist "%MSBUILD%" goto :EOF

    echo Building DSI Revit Toolkit with Visual Studio version %VSVER%
    cd ..
	if not (%1)==() call :Build %1
	if not (%1)==() goto :DONE

	call :Build 2018
	call :Build 2019
	call :Build 2020
	call :Build 2021
	call :Build 2022
	call :Build 2023
	
	:DONE
	cd install
	
goto :EOF

:Build
    echo Building %1
    del .\src\bin\%1\Debug\DSIRevitToolkit.dll 1>nul 2>nul
    del .\src\bin\%1\Release\DSIRevitToolkit.dll 1>nul 2>nul
	
    echo. > debug%1.txt
    echo. > release%1.txt
	"%MSBUILD%" dsi-toolkit.sln "/property:Configuration=Debug (Revit %1)" /p:Platform=x64 >> debug%1.txt
    if exist ".\src\bin\x64\Debug (Revit %1)\DSIRevitToolkit.dll"       echo          %1 Debug   build: SUCCESS
    if not exist ".\src\bin\x64\Debug (Revit %1)\DSIRevitToolkit.dll"   echo          %1 Debug   build: FAILED to create bin\x64\Debug (Revit %1)\Debug\DSIRevitToolkit.dll
    if not exist ".\src\bin\x64\Debug (Revit %1)\DSIRevitToolkit.dll"   type debug%1.txt | find "Error"
		
	"%MSBUILD%" dsi-toolkit.sln  "/property:Configuration=Release (Revit %1)" /p:Platform=x64 >> release%1.txt
    if exist ".\src\bin\x64\Release (Revit %1)\DSIRevitToolkit.dll"     echo          %1 Release build: SUCCESS
    if not exist ".\src\bin\x64\Release (Revit %1)\DSIRevitToolkit.dll" echo          %1 Release build: FAILED to create bin\x64\Release (Revit %1)\DSIRevitToolkit.dll
	if not exist ".\src\bin\x64\Release (Revit %1)\DSIRevitToolkit.dll" type release%1.txt | find "Error"
	
	del debug%1.txt 1>nul 2>nul
	del release%1.txt 1>nul 2>nul
	
goto :EOF
