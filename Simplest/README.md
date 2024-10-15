# Simplest

Простейший веб-сервер для раздачи статичного контента. Управляется конфигурационным файлом `appsettings.json` и/или через переменные окружения и/или из командной строки.

В конфигурационном файле:

```json
{
  "contentRoot": "D:\\Web",
  "wwwroot": "pics",
  "Urls": "http://localhost:4123"
}
```

Переменные окружения:

```
ASPNETCORE_CONTENTROOT=/var/pics
ASPNETCORE_WEBROOT=/var/pics/wwwroot
ASPNETCORE_URLS=http://localhost:4123
```

В командной строке:

```
Simplest СontentRoot=D:\Web wwwroot=pics Urls=http://localhost:4123
```

Значения по умолчанию:

* `contentRoot`: папка веб-сервера,
* `wwwroot`: `"wwwroot"`,
* `Urls`: `"http://localhost:5000"`.
