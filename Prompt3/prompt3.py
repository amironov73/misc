import argparse
import sys
from mutagen.id3 import ID3, TXXX, ID3NoHeaderError

def main():
    parser = argparse.ArgumentParser(description="Встраивание промпта в MP3 файл.")
    parser.add_argument("file", help="Путь к MP3 файлу")
    parser.add_argument("prompt", help="Текст промпта для встраивания")
    parser.add_argument("--replace", action="store_true", help="Заменить существующий промпт, если он уже есть")

    args = parser.parse_args()

    try:
        # Пробуем загрузить ID3 теги, если файла нет или он без тегов — создаем объект
        try:
            audio = ID3(args.file)
        except ID3NoHeaderError:
            audio = ID3()

        # Ищем существующий фрейм TXXX с описанием 'Prompt'
        existing_prompt = audio.getall("TXXX:Prompt")

        if existing_prompt and not args.replace:
            print(f"ОШИБКА: В файле уже содержится промпт: '{existing_prompt[0].text[0]}'")
            print("Используйте флаг --replace для перезаписи.")
            sys.exit(1)

        # Добавляем или заменяем тег
        # encoding=3 означает UTF-8
        audio.add(TXXX(encoding=3, desc='Prompt', text=args.prompt))

        # Сохраняем изменения (остальные теги и обложка сохраняются автоматически)
        audio.save(args.file)
        print(f"Готово! Промпт успешно {'обновлен' if existing_prompt else 'встроен'}.")

    except FileNotFoundError:
        print(f"ОШИБКА: Файл '{args.file}' не найден.")
        sys.exit(1)
    except Exception as e:
        print(f"Произошла непредвиденная ошибка: {e}")
        sys.exit(1)

if __name__ == "__main__":
    main()
