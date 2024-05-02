#!/usr/bin/env python3


"""
Простая программа для перевода тегов распознанных изображений
на русский язык.
"""

import io
import os
from typing import List, Dict
import requests
import spacy


# Путь, по которому расположены исходные файлы
ROOT_PATH = 'H:\\Images2'

# Сетевой таймаут в секундах
TIMEOUT = 3


class MicroGPT:
    """
    Вызов локальной LLM.
    """

    def __init__(self, base_url: str):
        self.base_url = base_url

    def models(self) -> List:
        """
        Получение списка моделей.
        :return: Список моделей.
        """
        response = requests.get(self.base_url + "/v1/models", timeout=TIMEOUT).json()
        return response["data"]

    def completions(self, system_prompt: str, user_prompt: str,
                    temperature: float = 0.8) -> Dict:
        """
        Получение продолжения текста.
        :param system_prompt: Системная подсказка.
        :param user_prompt: Пользовательская подсказка.
        :param temperature: Температура.
        :return: Ответ LLM с продолжениями.
        """
        payload = {"messages": [
            {"role": "system", "content": system_prompt},
            {"role": "user", "content": user_prompt}
        ],
            "temperature": temperature,
            "stream": False
        }
        response = requests.post(self.base_url + "/v1/chat/completions",
                                 json=payload, timeout=TIMEOUT).json()
        return response


gpt = MicroGPT('http://127.0.0.1:1234')
ru = spacy.load("ru_core_news_lg")

dictionary = {}
with io.open('dictionary.txt', 'r', encoding='utf-8') as file:
    dictionary[file.readline().strip()] = file.readline().strip()


def translate(source_text: str) -> str:
    """
    Перевод текста на русский язык с помощью локальной LLM.
    """

    system_prompt = "You literally translate text from English into Russian. Be concise."
    user_prompt = "Дословно переведи на русский язык: " + source_text
    temperature = 0.1
    completions = gpt.completions(system_prompt, user_prompt, temperature)
    return completions["choices"][0]["message"]["content"]


def smart_translate(source_text: str) -> str:
    """
    Умный перевод текста, учитывающий закидоны LLM.
    """
    result = translate(source_text)
    result = result.replace('\n', ' ').strip(' .')
    return result


def fast_translate(source_text: str) -> str:
    """
    Быстрый перевод текста с английского на русский.
    :param source_text: Текст для перевода.
    :return: Переведенный текст.
    """
    result = []
    words = source_text.split(', ')
    for word in words:
        if word not in dictionary:
            dictionary[word] = smart_translate(word)

        if word not in ['.', '-']:
            result.append(dictionary[word])

    return ', '.join(result)


def shorten(text: str) -> str:
    """
    Удаляем из текста ненужные фрагменты
    """
    text = text.replace('a black and white photo of ', '')
    text = text.replace('a black and white photograph of ', '')
    return text


class PhotoImage:
    """
    Файл с описаниями фотографий
    """

    short_source: str
    full_source: str
    first_line_en: str
    first_line_ru: str
    second_line_en: str
    second_line_ru: str
    tags_ru: List[str]

    def read(self) -> None:
        """
        Считывания описания фотографии из файла.
        """
        with io.open(self.full_source, 'r', encoding='utf-8') as f:
            self.first_line_en = shorten(f.readline().strip())
            self.second_line_en = f.readline().strip()


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
            if ext in ['.txt']:
                img = PhotoImage()
                img.short_source = filename
                img.full_source = str(os.path.join(path, filename))
                img.read()
                result.append(img)
    return result


#####################################################

with io.open('bad_tags.txt', 'r', encoding='utf-8') as file:
    bad_tags = [line.strip() for line in file.readlines()]


def get_tags(text: str) -> List[str]:
    """
    Разбор текста на элементы (теги).
    :param text: Текст для разбора.
    :return: Список тегов.
    """
    result = [t.strip() for t in text.split(',')]
    result = [t for t in result if t not in bad_tags]
    return result


counter = {}
all_images = find_all_images()

with io.open('tags.txt', 'w', encoding='utf-8') as output:
    for one in all_images:
        one.first_line_ru = (smart_translate(one.first_line_en)
                             .lower().strip('.'))
        one.second_line_ru = fast_translate(', '
                                            .join(get_tags(one.second_line_en))).lower().strip('.')
        doc = ru(one.first_line_ru)
        one.tags_ru = ([str(token.lemma_) for token in doc if
                        token.pos_ in ['NOUN', 'ADJ']] +
                       [item.strip(' "') for item in
                        one.second_line_ru.split(',')])

        for tag in one.tags_ru:
            if tag not in counter:
                counter[tag] = 0
            counter[tag] = counter[tag] + 1

        output_line = (one.short_source + '\n' +
                       one.first_line_en + '\n' +
                       one.second_line_en + '\n' +
                       ', '.join(one.tags_ru) + '\n' +
                       '\n')
        output.write(output_line)

keys = sorted(dictionary.keys())
with io.open('dictionary2.txt', 'w', encoding='utf-8') as file:
    for key in keys:
        file.write(key)
        file.write(dictionary[key])

keys = sorted(counter.keys())
with io.open('counter.txt', 'w', encoding='utf-8') as file:
    for key in keys:
        file.write(f"{key}\t{counter[key]}")
