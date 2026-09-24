@echo off

rem Because of Miscroslop shinanigans, we need to restore the project for each runtime identifier (RID) separately.
rem This is because the RID-specific assets are not restored by default when creating a package app with WAP.
rem See: https://blog.hn-pgtech.com/2025-08-01/


dotnet restore "..\Game Pass Save Tranfer\Xbox Game Save Exporter.WPF.csproj" -r win-x86 /p:BaseIntermediateOutputPath="obj\wappublish\win-x86\"
dotnet restore "..\Game Pass Save Tranfer\Xbox Game Save Exporter.WPF.csproj" -r win-x64 /p:BaseIntermediateOutputPath="obj\wappublish\win-x64\"
rem dotnet restore "..\Game Pass Save Tranfer\Xbox Game Save Exporter.WPF.csproj" -r win-arm /p:BaseIntermediateOutputPath="obj\wappublish\win-arm\"
rem dotnet restore "..\Game Pass Save Tranfer\Xbox Game Save Exporter.WPF.csproj" -r win-arm64 /p:BaseIntermediateOutputPath="obj\wappublish\win-arm64\"