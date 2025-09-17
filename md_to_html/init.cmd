@echo off

%LOCALAPPDATA%\Programs\Python\Python310\python -m venv .venv
.venv\Scripts\pip install -r requirements.txt
