; Inno Setup Script: http://www.jrsoftware.org/
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Yabe"
#define MyAppVersion "2.1.0"
#define MyAppPublisher "Yabe Authors"
#define MyAppURL "http://sourceforge.net/projects/yetanotherbacnetexplorer"
#define MyAppExeName "Yabe.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{F8639277-80EB-4EC9-AE36-D4BF2115ABFA}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DefaultGroupName={#MyAppName}
LicenseFile=..\..\Docs\MIT_license.txt
OutputBaseFilename=SetupYabe_v{#MyAppVersion}
Compression=lzma
SolidCompression=yes
ArchitecturesInstallIn64BitMode=x64 

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"
Name: "czech"; MessagesFile: "compiler:Languages\Czech.isl"
Name: "danish"; MessagesFile: "compiler:Languages\Danish.isl"
Name: "dutch"; MessagesFile: "compiler:Languages\Dutch.isl"
Name: "finnish"; MessagesFile: "compiler:Languages\Finnish.isl"
Name: "french"; MessagesFile: "compiler:Languages\French.isl"
Name: "german"; MessagesFile: "compiler:Languages\German.isl"
Name: "hebrew"; MessagesFile: "compiler:Languages\Hebrew.isl"
Name: "italian"; MessagesFile: "compiler:Languages\Italian.isl"
Name: "japanese"; MessagesFile: "compiler:Languages\Japanese.isl"
Name: "norwegian"; MessagesFile: "compiler:Languages\Norwegian.isl"
Name: "polish"; MessagesFile: "compiler:Languages\Polish.isl"
Name: "portuguese"; MessagesFile: "compiler:Languages\Portuguese.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "slovenian"; MessagesFile: "compiler:Languages\Slovenian.isl"
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"
Name: "turkish"; MessagesFile: "compiler:Languages\Turkish.isl"
Name: "ukrainian"; MessagesFile: "compiler:Languages\Ukrainian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "..\..\Yabe\bin\Debug\Yabe.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\Yabe\bin\Debug\Plugins\CheckReliability.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion
Source: "..\..\Yabe\bin\Debug\Plugins\CheckStatusFlags.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion
Source: "..\..\Yabe\bin\Debug\Plugins\ListAnalog_Values.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion
Source: "..\..\Yabe\bin\Debug\Plugins\ListCOV_Increment.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion
Source: "..\..\Yabe\bin\Debug\Plugins\ListOutOfService.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion
Source: "..\..\Yabe\bin\Debug\Plugins\GlobalCommander.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion
Source: "..\..\Yabe\bin\Debug\Plugins\FindPriorities.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion
Source: "..\..\Yabe\bin\Debug\Plugins\FindPrioritiesGlobal.dll"; DestDir: "{app}\Plugins"; Flags: ignoreversion
Source: "..\..\Yabe\bin\Debug\README.Txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\Docs\history.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\Docs\MIT_license.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\Docs\ZedGraph Calendar SharpPcap License-LGPL.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\Docs\Treeview_license.txt"; DestDir: "{app}"; Flags: ignoreversion

Source: "..\..\Yabe\bin\Debug\ReadSinglePropDescr.Xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\Yabe\bin\Debug\SimplifiedViewFilter.Xml"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\Yabe\bin\Debug\YabeMenuCmd.Txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\Yabe\bin\Debug\RecipeSample.csv"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\Yabe\bin\Debug\AdvertiseSample.cov"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\Yabe\bin\Debug\VendorPropertyMapping.csv"; DestDir: "{app}"; Flags: ignoreversion

Source: "..\..\DemoServer\bin\Debug\DemoServer.exe"; DestDir: "{app}\AddOn"; Flags: ignoreversion
Source: "..\..\DemoServer\bin\Debug\DeviceStorage.Xml"; DestDir: "{app}\AddOn"; Flags: ignoreversion
                           
Source: "..\..\CodeExamples\Bacnet.Room.Simulator\bin\Debug\Bacnet.Room.Simulator.exe"; DestDir: "{app}\AddOn"; Flags: ignoreversion
Source: "..\..\CodeExamples\Bacnet.Room.Simulator\bin\Debug\Bacnet.Room.Simulator.exe.config"; DestDir: "{app}\AddOn"; Flags: ignoreversion
Source: "..\..\CodeExamples\Bacnet.Room.Simulator\Readme.txt"; DestDir: "{app}\AddOn"; Flags: ignoreversion

Source: "..\..\CodeExamples\Wheather2_to_Bacnet\bin\Debug\Wheather2_to_Bacnet.exe"; DestDir: "{app}\AddOn"; Flags: ignoreversion
Source: "..\..\CodeExamples\Wheather2_to_Bacnet\Readme.txt"; DestName:"ReadmeWheather2.txt"; DestDir: "{app}\AddOn"; Flags: ignoreversion
Source: "..\..\CodeExamples\Wheather2_to_Bacnet\Wheather2config.reg"; DestDir: "{app}\AddOn"; Flags: ignoreversion

Source: "..\..\Mstp.BacnetCapture\bin\Debug\Mstp.BacnetCapture.exe"; DestDir: "{app}\AddOn"; Flags: ignoreversion

Source: "..\..\Yabe\Common Files\Yabe.p12"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\Yabe\Common Files\TestHub.crt"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\Yabe\Common Files\BACnetSCConfig.config"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\Yabe"; Filename: "{app}\Yabe.Exe"

Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"

Name: "{group}\Doc\Yabe Directory"; Filename : "{app}"
Name: "{group}\Doc\Yabe Website"; Filename : "{#MyAppURL}"
Name: "{group}\Doc\Readme"; Filename: "{app}\Readme.txt"
Name: "{group}\Doc\Full source code"; Filename: "http://sourceforge.net/p/yetanotherbacnetexplorer/code/HEAD/tarball?path=/trunk"
   
Name: "{group}\AddOn\DemoServer"; Filename: "{app}\AddOn\DemoServer.Exe"
Name: "{group}\AddOn\Mstp.BacnetCapture"; Filename: "{app}\AddOn\Mstp.BacnetCapture.exe"

Name: "{group}\AddOn\Bacnet.Room.Simulator"; Filename: "{app}\AddOn\Bacnet.Room.Simulator.exe"
Name: "{group}\AddOn\RoomSimulatorReadme"; Filename: "{app}\AddOn\Readme.txt"

Name: "{group}\AddOn\Wheather2_to_Bacnet"; Filename: "{app}\AddOn\Wheather2_to_Bacnet.exe"; Parameters: "ConsoleMode"
Name: "{group}\AddOn\Wheather2Readme"; Filename: "{app}\AddOn\ReadmeWheather2.txt"

Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, "&", "&&")}}"; Flags: nowait postinstall skipifsilent


