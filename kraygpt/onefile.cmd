@echo off

if "L%1L" == "LL" exit

.venv\Scripts\python.exe onefile.py %1
