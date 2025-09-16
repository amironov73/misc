# Конвертер Markdown в HTML

Скрипт для конвертации файлов из формата Markdown в HTML с поддержкой различных расширений и настроек.

## Установка зависимостей

```bash
pip install markdown
```

Для дополнительных возможностей (подсветка синтаксиса):
```bash
pip install pygments
```

## Использование

### Базовое использование
```bash
python md_to_html.py input.md
```

### Указание выходного файла
```bash
python md_to_html.py input.md -o output.html
```

### Создание полной HTML страницы с CSS
```bash
python md_to_html.py example.md --full-page --title "Моя страница" --css styles.css
```

### Использование конкретных расширений
```bash
python md_to_html.py input.md -e extra tables codehilite
```

## Параметры командной строки

- `input_file` - Путь к входному файлу Markdown (обязательный)
- `-o, --output` - Путь к выходному HTML файлу (опционально)
- `-e, --extensions` - Список расширений Markdown (по умолчанию: extra, codehilite, toc)
- `--encoding` - Кодировка файлов (по умолчанию: utf-8)
- `--full-page` - Создать полную HTML страницу с DOCTYPE, head и body
- `--title` - Заголовок HTML страницы (используется только с --full-page)
- `--css` - Путь к CSS файлу для включения в HTML
- `-q, --quiet` - Тихий режим (не выводить информационные сообщения)

## Поддерживаемые расширения Markdown

- `extra` - Набор дополнительных возможностей (таблицы, сноски и др.)
- `codehilite` - Подсветка синтаксиса кода
- `toc` - Автоматическое создание таблицы содержания
- `tables` - Поддержка таблиц
- `fenced_code` - Блоки кода в тройных кавычках
- `footnotes` - Поддержка сносок
- `attr_list` - Атрибуты для элементов
- `def_list` - Списки определений
- `abbr` - Сокращения

## Примеры использования

1. **Простая конвертация:**
   ```bash
   python md_to_html.py document.md
   ```

2. **Конвертация с полной HTML страницей:**
   ```bash
   python md_to_html.py document.md --full-page --title "Мой документ"
   ```

3. **Конвертация с CSS стилями:**
   ```bash
   python md_to_html.py document.md --full-page --css my-styles.css
   ```

4. **Тихий режим с конкретными расширениями:**
   ```bash
   python md_to_html.py document.md -q -e extra tables
   ```

5. **Указание кодировки:**
   ```bash
   python md_to_html.py document.md --encoding cp1251
   ```

## Структура проекта

```
├── md_to_html.py    # Основной скрипт
├── example.md       # Пример Markdown файла
├── styles.css       # Пример CSS стилей
└── README.md        # Этот файл
```

## Возможности

- ✅ Конвертация базового Markdown в HTML
- ✅ Поддержка расширений Python-Markdown
- ✅ Создание полных HTML страниц
- ✅ Включение CSS стилей
- ✅ Настройка кодировки файлов
- ✅ Подсветка синтаксиса кода
- ✅ Автоматическое создание таблицы содержания
- ✅ Поддержка таблиц и других расширенных элементов
- ✅ Обработка ошибок и информативные сообщения

## Требования

- Python 3.6+
- markdown >= 3.0
- pygments (опционально, для подсветки синтаксиса)
