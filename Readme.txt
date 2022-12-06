-----------------------------------------------------------------------------------
DSI Revit Toolkit

This readme.txt will tell you how to build and release this software.

GTP Sevices, 2022
SSR
-----------------------------------------------------------------------------------


1. DSI Toolkit is a program for use in Revit

2. GTP Services helped do development on this tool in 2022 (creating this file)

3. Assuming you have the ZIP containing all the source code for the DSI Toolkit:
   Place the DSI toolkit in some code path.  In the case of GTP, the code 
   is located here:
   
         C:\repos\DSI\revit-toolkit\
		 												  
				11/21/2022  09:43 PM    <DIR>          .
				11/21/2022  09:43 PM    <DIR>          ..
				11/01/2022  05:33 PM               568 .editorconfig
				11/01/2022  05:33 PM             2,518 .gitattributes
				11/01/2022  05:33 PM             5,778 .gitignore
				11/21/2022  04:50 PM            10,585 dsi-toolkit.sln
				11/19/2022  11:12 AM    <DIR>          ExternalLibraries
				11/21/2022  12:28 PM    <DIR>          install
				11/09/2022  06:18 PM    <DIR>          packages
				11/21/2022  09:43 PM              1987 Readme.txt                   ***** This file
				11/21/2022  06:15 PM    <DIR>          src


4.  Inside the revit-toolkit\install directory, you will find the build scripts.

5.  Directory of C:\repos\DSI\revit-toolkit\install

				11/21/2022  10:30 PM    <DIR>          .
				11/21/2022  10:30 PM    <DIR>          ..
				11/21/2022  05:06 PM             2,518 build.cmd
				11/21/2022  10:36 PM             9,701 CreateInstall.cmd
				11/21/2022  01:45 PM               201 excludeFiles.txt
				11/21/2022  07:43 PM             6,223 install.bat
				11/21/2022  06:24 PM                76 Install.message.txt
				11/21/2022  10:31 PM                62 InstallTitle.txt
				11/21/2022  05:59 PM             1,908 nodsi.cmd
				11/18/2022  05:00 PM             2,222 sign.cmd
				
				
6. Optional: If you want to sign your DLLs and EXE files, edit sign.cmd with the path to your certificates and a password file

7. Open a command window and 

	cd C:\repos\DSI\revit-toolkit\install\
	
   (or wherever your sourse path is: cd <yourpath>\install

8. Run CreateInstall.cmd
	
   a. First Install Visual Studio on your machine (the build.cmd script supports vs 2022, Professional 2017, or Community 2019).
      Whatever version you have, you can edit build.cmd with the location of your MSBUILD.exe program.
   b. CreateInstall.cmd will build the software and create the self-extracting EXE.
      Run CreateInstall.cmd twice. Once to create the 32bit installer, and once for 64bit: 
		CreateInstall -32
		CreateInstall -64   		
   c. The resulting EXEs are two installers:      
	    DSIRevitToolkit32bit.exe
	    DSIRevitToolkit64bit.exe	  
	  These are the installers to run on the target machines.	  
   d. When these installers run, they install the DSI toolkit to:   
        C:\Program Files (x86)\DSI\

9. This software uses a licesned version of WinZip self extracting EXE creator.

   http://winzip.com/en/product/self-extractor/

10. Developer notes:
    a. If you are a programmer, you can build the software by running:
          build.cmd 
	b. You can then have all the Revit manifest files point to your build by running
	      install -dev		 
	c. You can clean out your binaries and erase the manifests by running:
	      nodsip.cmd
	   This is useful if you are trying to isolate any copy or build problems.
	   	   
11.	Adding new versions of Revit
    At the time of this writing, Revit 2023 was the latest version of Revit.
	a. If you want to add Revit 24, update all the .CMD files
	b. Edit the .sln file for the project, and edit the .csproj
	c. Install Revit 2024, and copy the DLLs to:	
	
           C:\repos\DSI\revit-toolkit\ExternalLibraries\2024\
		   
    ExternalLibraries is a directory that contains all the needed Revit DLLs
	