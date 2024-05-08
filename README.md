# ClockWidget for WigiDash

## Pre-requisites

- Visual Studio 2022
- WigiDash Manager (https://wigidash.com/)

## Getting started

1. Clone this repository
2. Open ClockWidget.csproj in Visual Studio
3. Resolve the dependancy for WigiDashWidgetFramework under References by adding a reference to 
```
C:\Program Files (x86)\G.SKILL\WigiDash Manager\WigiDashWidgetFramework.dll
```
4. Open Project properties -> Build Events and add this to Post-build event command line:
```
rd /s /q "C:\Users\elmor\AppData\Roaming\G.SKILL\WigiDashManager\Widgets\$(TargetName)\"
xcopy "$(TargetDir)\" "C:\Users\elmor\AppData\Roaming\G.SKILL\WigiDashManager\Widgets\$(TargetName)\" /F /Y /E /H /C /I
```
5. Open Project properties -> Debug and select Start external program: "C:\Program Files (x86)\G.SKILL\WigiDash Manager\WigiDashManager.exe".
6. Start debugging the project, and it should launch WigiDash Manager with your Widget loaded and debuggable.
