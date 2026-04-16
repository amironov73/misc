@echo off

start /min C:\Users\amiro\AppData\Local\Programs\Python\Python310\python.exe -m http.server 8000

timeout /t 1 >nul

start http://localhost:8000/index.html
