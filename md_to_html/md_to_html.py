#!/usr/bin/env python3
"""
Скрипт для конвертации файлов Markdown в HTML
Поддерживает различные расширения и настройки
"""

import argparse
import markdown
import sys
import os
from pathlib import Path


def setup_parser():
    """Настройка парсера аргументов командной строки"""
    parser = argparse.ArgumentParser(
        description='Конвертер файлов Markdown в HTML',
        epilog='Пример использования: python md_to_html.py input.md -o output.html',
        formatter_class=argparse.RawDescriptionHelpFormatter
    )

    # Позиционный аргумент - входной файл
    parser.add_argument(
        'input_file',
        help='Путь к входному файлу Markdown'
    )

    # Опциональный аргумент - выходной файл
    parser.add_argument(
        '-o', '--output',
        help='Путь к выходному HTML файлу (по умолчанию: input_file.html)'
    )

    # Расширения Markdown
    parser.add_argument(
        '-e', '--extensions',
        nargs='*',
        default=['extra', 'codehilite', 'toc'],
        help='Список расширений Markdown (по умолчанию: extra, codehilite, toc)'
    )

    # Кодировка файлов
    parser.add_argument(
        '--encoding',
        default='utf-8',
        help='Кодировка файлов (по умолчанию: utf-8)'
    )

    # Включить полную HTML страницу
    parser.add_argument(
        '--full-page',
        action='store_true',
        help='Создать полную HTML страницу с DOCTYPE, head и body'
    )

    # Заголовок страницы
    parser.add_argument(
        '--title',
        help='Заголовок HTML страницы (используется только с --full-page)'
    )

    # CSS стили
    parser.add_argument(
        '--css',
        help='Путь к CSS файлу для включения в HTML'
    )

    # Тихий режим
    parser.add_argument(
        '-q', '--quiet',
        action='store_true',
        help='Не выводить информационные сообщения'
    )

    return parser


def load_css(css_path):
    """Загрузка CSS файла"""
    try:
        with open(css_path, 'r', encoding='utf-8') as f:
            return f.read()
    except FileNotFoundError:
        print(f"Внимание: CSS файл '{css_path}' не найден", file=sys.stderr)
        return ""
    except Exception as e:
        print(f"Ошибка при чтении CSS файла: {e}", file=sys.stderr)
        return ""


def create_full_html(content, title=None, css_content=None):
    """Создание полной HTML страницы"""
    if title is None:
        title = "Конвертированная страница"

    css_block = ""
    if css_content:
        css_block = f"<style>\n{css_content}\n</style>"

    return f"""<!DOCTYPE html>
<html lang="ru">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>{title}</title>
    {css_block}
</head>
<body>
{content}
</body>
</html>"""


def convert_markdown_to_html(input_file, output_file, extensions, encoding, 
                           full_page, title, css_path, quiet):
    """Основная функция конвертации"""

    # Проверка существования входного файла
    if not os.path.exists(input_file):
        print(f"Ошибка: файл '{input_file}' не найден", file=sys.stderr)
        return False

    # Определение выходного файла
    if output_file is None:
        input_path = Path(input_file)
        output_file = input_path.with_suffix('.html')

    try:
        # Чтение Markdown файла
        with open(input_file, 'r', encoding=encoding) as f:
            markdown_content = f.read()

        if not quiet:
            print(f"Читаем файл: {input_file}")

        # Создание экземпляра Markdown с расширениями
        md = markdown.Markdown(extensions=extensions)

        # Конвертация в HTML
        html_content = md.convert(markdown_content)

        # Обработка полной страницы
        if full_page:
            css_content = None
            if css_path:
                css_content = load_css(css_path)

            if title is None:
                # Попытка извлечь заголовок из первой строки
                first_line = markdown_content.split('\n')[0].strip()
                if first_line.startswith('#'):
                    title = first_line.lstrip('#').strip()
                else:
                    title = Path(input_file).stem

            html_content = create_full_html(html_content, title, css_content)

        # Запись HTML файла
        with open(output_file, 'w', encoding=encoding) as f:
            f.write(html_content)

        if not quiet:
            print(f"HTML файл создан: {output_file}")

        return True

    except UnicodeDecodeError as e:
        print(f"Ошибка кодировки при чтении файла: {e}", file=sys.stderr)
        print(f"Попробуйте указать другую кодировку с помощью --encoding", file=sys.stderr)
        return False
    except Exception as e:
        print(f"Ошибка при конвертации: {e}", file=sys.stderr)
        return False


def main():
    """Главная функция"""
    parser = setup_parser()
    args = parser.parse_args()

    # Конвертация
    success = convert_markdown_to_html(
        input_file=args.input_file,
        output_file=args.output,
        extensions=args.extensions,
        encoding=args.encoding,
        full_page=args.full_page,
        title=args.title,
        css_path=args.css,
        quiet=args.quiet
    )

    # Выход с соответствующим кодом
    sys.exit(0 if success else 1)


if __name__ == '__main__':
    main()
