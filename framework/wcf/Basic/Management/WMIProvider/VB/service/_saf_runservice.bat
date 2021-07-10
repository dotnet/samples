@echo off
DummyExe.exe service
cscript /nologo EnumerateServices.js
cscript /nologo EnumerateCustomObjects.js
