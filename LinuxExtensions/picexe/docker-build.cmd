@echo off

set IMAGE_NAME=my-builder-image
set EXE_NAME=picexe
set SO_NAME=libpic.so

del %SO_NAME% 2> nul
del %EXE_NAME% 2> nul

:: 1. Сборка (использует кэш, если Dockerfile не менялся)
docker build -t %IMAGE_NAME% .
if %ERRORLEVEL% neq 0 (
    echo [ERROR] Build failed! Check Dockerfile or source code.
    pause
    exit /b %ERRORLEVEL%
)

:: 2. Копирование файла через временный контейнер-«пустышку»
:: Мы просто запускаем команду 'cat', перенаправляя вывод в файл на хосте
docker run --rm --entrypoint cat %IMAGE_NAME% /app/%EXE_NAME% > %EXE_NAME%
docker run --rm --entrypoint cat %IMAGE_NAME% /app/%SO_NAME% > %SO_NAME%

:: Образ НЕ удаляем, чтобы в следующий раз сборка заняла 1 секунду
echo Done!