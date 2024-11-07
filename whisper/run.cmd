@echo off

call environment.cmd

mkdir chunks >nul 2> nul
python.exe source\app.py %1 %2
