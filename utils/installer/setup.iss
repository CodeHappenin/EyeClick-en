; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "EyeClick"
#define MyAppVersion "1.3"
#define MyAppExeName "Eyeclick.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{03A6A859-E7AE-4FB9-8AA1-DE003A3A89EC}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
DefaultDirName={pf}\{#MyAppName}
DisableDirPage=yes
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputBaseFilename=setup
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}";

[Files]
Source: "C:\Users\computer\Documents\Visual Studio 2010\Projects\eyeclick-en\facereco_test\bin\Release\EyeClick.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\computer\Documents\Visual Studio 2010\Projects\eyeclick-en\facereco_test\bin\Release\libpxcclr.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\computer\Documents\Visual Studio 2010\Projects\eyeclick-en\utils\intel_pc_sdk_runtime_ia32_7383.msi"; DestDir: "{app}"; Flags: ignoreversion
Source: "C:\Users\computer\Documents\Visual Studio 2010\Projects\eyeclick-en\utils\dotNetFx40_Full_setup.exe"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Run]
Filename: "msiexec.exe"; Parameters: "/i""{app}\intel_pc_sdk_runtime_ia32_7383.msi""/qb"
;Filename: "{app}\dotNetFx40_Full_setup.exe"; Parameters: "/passive /norestart"

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

