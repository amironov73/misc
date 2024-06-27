#!/usr/bin/env python3

"""
Простая программа для распознания изображений
на русском языке с помощью GPT-4o.
"""

import os
import io
from datetime import datetime
from typing import List
from pathlib import Path

from _common import initialize, recognize_image, get_caption

# Путь, по которому расположены исходные файлы
ROOT_PATH = 'E:\\Images'

# Префикс, добавляемый к имени файла
IMAGE_PATH_PREFIX = 'pic\\'


class PhotoImage:
    """
    Файл с описаниями фотографий
    """

    short_source: str
    full_source: str
    caption_en: str
    caption_ru: str
    tags_en: str
    tags_ru: str
    caption_ru: str

    def read(self) -> None:
        """
        Считывание описания фотографии из файла.
        """
        p = Path(self.full_source)
        text_file_name = p.with_suffix('.txt')
        with io.open(text_file_name, 'r', encoding='utf-8') as f:
            self.caption_en = f.readline().strip()
            self.tags_en = f.readline().strip()
            self.caption_ru = f.readline().strip()
            self.tags_ru = f.readline().strip()

    def write(self) -> None:
        """
        Сохранение описания фотографии в файл.
        """
        p = Path(self.full_source)
        text_file_name = p.with_suffix('.txt')
        with io.open(text_file_name, 'w', encoding='utf-8') as f:
            f.write(self.caption_en + '\n' +
                    self.tags_en + '\n' +
                    self.caption_ru + '\n' +
                    self.tags_ru + '\n'
                    )

    def __str__(self):
        return self.short_source


def find_all_images() -> List[PhotoImage]:
    """
    Чтение описаний для всех фотографий, какие только найдутся
    по пути `ROOT_PATH`.
    :return: Список найденных описаний.
    """
    result: List[PhotoImage] = []
    for path, _, files in os.walk(ROOT_PATH):
        for filename in files:
            ext = os.path.splitext(filename)[1].lower()
            if ext in ['.jpg']:
                img = PhotoImage()
                img.short_source = filename
                img.full_source = str(os.path.join(path, filename))
                img.read()
                result.append(img)
    return result


def translate_tags(tags_en: str) -> str:
    """
    Примитивный перевод тегов по словарю.
    :param tags_en: Теги на английском языке через запятую.
    :return: Теги на русском языке через запятую.
    """
    result = []
    words = tags_en.split(', ')
    for word in words:
        if word in dictionary:
            result.append(dictionary[word])
    return ', '.join(result)


#####################################################


initialize()

# считываем словарь
dictionary = {}
with io.open('dictionary.txt', 'r', encoding='utf-8') as file:
    while True:
        line1 = file.readline().strip()
        line2 = file.readline().strip()
        if not line1 or not line2:
            break
        dictionary[line1] = line2

all_images = find_all_images()  # все найденные по указанному пути картинки

# обрабатываем найденные подписи к картинкам и сохраняем их
with io.open('tags.txt', 'a', encoding='utf-8') as output:
    for one in all_images:
        print(one.short_source)

        if one.caption_ru:
            print(" => Already captioned")
            continue

        start_moment = datetime.now()  # для отслеживания времени исполнения
        one.tags_ru = translate_tags(one.tags_en)
        completion = recognize_image(one.full_source)
        one.caption_ru = get_caption(completion)
        elapsed = datetime.now() - start_moment
        one.write()
        print(' =>', elapsed)
        print(' =>', one.caption_ru)
        print(' =>', one.tags_ru)

        file_name = (IMAGE_PATH_PREFIX + one.full_source.removeprefix(ROOT_PATH + '\\')).lower()
        output_line = (file_name + '\n' +
                       one.caption_en + '\n' +
                       one.tags_en + '\n' +
                       one.caption_ru + '\n' +
                       one.tags_ru + '\n' +
                       'finish=' + completion.choices[0].finish_reason +
                       ', prompt=' + str(completion.usage.prompt_tokens) +
                       ', completion=' + str(completion.usage.completion_tokens) + '\n'
                       '\n'
                       )
        output.write(output_line)
        output.flush()  # чтобы не потерять последние записи
        print(" OK")

print("ALL DONE")
