﻿@echo off

dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=True