@echo off

rem Build store package to:
rem Xbox-Live-Save-Exporter\Xbox Live Save Exporter.WPF.WAP\AppPackages

rem You can replace the path below with your installed Visual Studio version (down to 14)
call "C:\Program Files\Microsoft Visual Studio\18\Community\Common7\Tools\VsDevCmd.bat"

msbuild "Xbox Game Save Exporter.WPF.WAP.wapproj" ^
/p:Configuration=Release ^
/p:AppxBundle=Always ^
/p:AppxBundlePlatforms="x86|x64" ^
/p:UapAppxPackageBuildMode=StoreUpload ^
/restore