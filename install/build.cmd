@echo off
rem ----------------------------------------------------------------------------------------------
rem  GTP STRATUS 2022 build script for DSI Revit ToolKit 
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
	call :Build 2018
	call :Build 2019
	call :Build 2020
	call :Build 2021
	call :Build 2022
	call :Build 2023
	cd install
	
goto :EOF

:Build
    echo Building %1
    del .\src\bin\%1\Debug\DSIRevitToolkit.dll 1>nul 2>nul
    del .\src\bin\%1\Release\DSIRevitToolkit.dll 1>nul 2>nul
	
    echo. > debug%1.txt
    echo. > release%1.txt
	"%MSBUILD%" dsi-toolkit.sln "/property:Configuration=Debug (Revit %1)" >> debug%1.txt
    if exist .\src\bin\%1\Debug\DSIRevitToolkit.dll       echo          %1 Debug   build: SUCCESS
    if not exist .\src\bin\%1\Debug\DSIRevitToolkit.dll   echo          %1 Debug   build: FAILED to create bin\%1\Debug\DSIRevitToolkit.dll
    if not exist .\src\bin\%1\Debug\DSIRevitToolkit.dll   type debug%1.txt | find "Error"
		
	"%MSBUILD%" dsi-toolkit.sln  "/property:Configuration=Release (Revit %1)" >> release%1.txt
    if exist .\src\bin\%1\Release\DSIRevitToolkit.dll     echo          %1 Release build: SUCCESS
    if not exist .\src\bin\%1\Release\DSIRevitToolkit.dll echo          %1 Release build: FAILED to create bin\%1\Release\DSIRevitToolkit.dll
	if not exist .\src\bin\%1\Release\DSIRevitToolkit.dll type release%1.txt | find "Error"
	
	del debug%1.txt 1>nul 2>nul
	del release%1.txt 1>nul 2>nul
	
goto :EOF
