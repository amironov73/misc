#!/usr/bin/env python3

"""
Поиск несоответствий между tags.txt и текстовыми файлами в папке.
"""

import os

ROOT_PATH = 'E:\\Images'
LIST_FILE = 'tags-fv.txt'

# ищем файлы в папке
found_files = []
for path, _, files in os.walk(ROOT_PATH):
    for filename in files:
        ext = os.path.splitext(filename)[1].lower()
        if ext in ['.jpg']:
            full_path = str(os.path.join(path, filename)).removeprefix(ROOT_PATH + '\\')
            found_files.append(full_path)

print(f"Found {len(found_files)} files")

# считываем список
listed_files = []
with open(LIST_FILE, 'r', encoding='utf-8') as f:
    while True:
        line = f.readline().strip()
        if not line:
            break
        listed_files.append(line)
        f.readline()  # 1
        f.readline()  # 2
        f.readline()  # 3
        f.readline()  # 4
        f.readline()  # 5
        f.readline()  # 6

print(f"Listed {len(listed_files)} files")

for found_file in found_files:
    if found_file not in listed_files:
        print(found_file)
