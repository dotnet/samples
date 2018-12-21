@echo off

SET SRCDIR=%~dp0src
SET OUTDIR=%~dp0bin\windows

mkdir %OUTDIR%

REM Build managed component
echo Building Managed Library
dotnet publish --self-contained -r win10-x64 %SRCDIR%\ManagedLibrary\ManagedLibrary.csproj -o %OUTDIR%

REM Build native component
cl.exe %SRCDIR%\SampleHost.cpp /Fo%OUTDIR%\ /Fd%OUTDIR%\SampleHost.pdb /EHsc /Od /GS /sdl /Zi /D "WINDOWS" /link /out:%OUTDIR%\SampleHost.exe
