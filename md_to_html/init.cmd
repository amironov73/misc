@echo off

%LOCALAPPDATA%\Python\Python310\python -m venv .venv
.venv\Scripts\pip install -r requirements.txt
