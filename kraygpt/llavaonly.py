#!/usr/bin/env python3

"""
Собираем только английский текст.
"""

import io
import os
from pathlib import Path

# Путь, по которому расположены исходные файлы
ROOT_PATH = 'C:\\Images\\fn'

# Префикс, добавляемый к имени файла
IMAGE_PATH_PREFIX = 'pic\\fn\\'

#####################################################

with io.open('tags.txt', 'a', encoding='utf-8') as output:
    for path, _, files in os.walk(ROOT_PATH):
        for filename in files:
            ext = os.path.splitext(filename)[1].lower()
            if ext in ['.jpg']:
                text_name = Path(str(os.path.join(path, filename))).with_suffix('.txt')
                with io.open(text_name,  'r', encoding='utf-8') as f:
                    text = f.readline().strip()
                image_name = (IMAGE_PATH_PREFIX + str(os.path.join(path, filename))
                              .removeprefix(ROOT_PATH + '\\')).lower()
                output.write(image_name + '\n' + text + '\n')
