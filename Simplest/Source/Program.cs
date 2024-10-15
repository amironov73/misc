// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

// ReSharper disable CheckNamespace

var builder = WebApplication.CreateSlimBuilder (args);

// Пересобираем конфигурацию, чтобы можно было задать параметры из командной строки
var configuration = builder.Configuration;
configuration.Sources.Clear();
configuration.AddCommandLine (args);
configuration.AddEnvironmentVariables();
configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

var app = builder.Build();

// Как настроить из командной строки:
// ./Simplest ContentRoot=D:\Web wwwroot=webroot Urls=http://localhost:4123

// Как настроить с помощью переменных окружения:
// ASPNETCORE_CONTENTROOT=/var/pics ./Simplest

// Нижеприведенный код не нужен, т. к. ASP.NET Core сам делает все необходимое
// var environment = app.Environment;
// var contentRoot = configuration["contentRoot"] ?? environment.ContentRootPath;
// var wwwroot = configuration["wwwroot"] ?? environment.WebRootPath;
// environment.ContentRootPath = contentRoot;
// environment.WebRootPath = wwwroot;

// При использовании UseDefaultFiles запросы к папке в wwwroot будут искать следующие файлы:
// default.htm
// default.html
// index.htm
// index.html
// app.UseDefaultFiles();

app.UseStaticFiles();

app.Run();
