#!/usr/bin/env python3


"""
Простая программа для перевода тегов распознанных изображений
на русский язык.
"""

import io
import os
from typing import List, Dict, Set
import requests
import spacy
import pymorphy2
from retry import retry
from datetime import datetime

# Путь, по которому расположены исходные файлы
ROOT_PATH = 'H:\\Images3'

# Сетевой таймаут в секундах
TIMEOUT = 100


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

    @retry(tries=15, delay=1, backoff=2)
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


start_moment = datetime.now()  # для отслеживания времени исполнения скрипта
gpt = MicroGPT('http://127.0.0.1:1234')
ru = spacy.load("ru_core_news_lg")
morph = pymorphy2.MorphAnalyzer()

# считываем словарь
dictionary = {}
with io.open('dictionary.txt', 'r', encoding='utf-8') as file:
    while True:
        line1 = file.readline().strip()
        line2 = file.readline().strip()
        if not line1 or not line2:
            break
        dictionary[line1] = line2


def translate(source_text: str) -> str:
    """
    Перевод текста на русский язык с помощью локальной LLM.
    Для перевода используется сервер LM Studio с загруженной моделью
    andrewcanis/c4ai-command-r-v01-GGUF/c4ai-command-r-v01-Q6_K.gguf

    Как запустить это дело:
    1. lms server start
    2. lms load "andrewcanis/c4ai-command-r-v01-GGUF/c4ai-command-r-v01-f16.gguf" --gpu=auto -y
    3. lms status (чтобы убедиться, что все работает)

    """

    system_prompt = ("You are a highly qualified translator of technical "
                     "texts. Literally translate subsequent texts from "
                     "English into Russian. Use simple language, short "
                     "sentences. Skip AI disclaimers, go straight "
                     "to the topic.")
    user_prompt = "Дословно переведи на русский язык: " + source_text
    # user_prompt = source_text
    temperature = 0.1
    completions = gpt.completions(system_prompt, user_prompt, temperature)
    return completions["choices"][0]["message"]["content"]


def filter_english_chars(text: str) -> str:
    """
    Фильтрация текста от неожиданных символов
    :param text: Текст, подлежащий фильтрации.
    :return: Отфильтрованный текст.
    """
    filtered = []
    for c in text:
        if c in ' ()' or ('a' <= c <= 'z') or ('A' <= c <= 'Z'):
            filtered.append(c)
    return ''.join(filtered)


def filter_russian_chars(text: str) -> str:
    """
    Фильтрация текста от неожиданных символов
    :param text: Текст, подлежащий фильтрации.
    :return: Отфильтрованный текст.
    """
    filtered = []
    for c in text:
        if c in ' ()-' or ('а' <= c <= 'я') or ('А' <= c <= 'Я'):
            filtered.append(c)
    return ''.join(filtered)


def smart_translate(source_text: str) -> str:
    """
    Умный перевод текста, учитывающий закидоны LLM.
    :param source_text: Исходный текст.
    :return: Переведенный текст.
    """
    print('.', end='')
    result = translate(source_text)
    result = result.replace('ё', 'е').replace('Ё', 'Е')
    result = result.replace('\n', ' ').strip(' .')
    result = filter_russian_chars(result)
    return result


def fast_translate(source_text: str) -> str:
    """
    Быстрый перевод текста с английского на русский.
    :param source_text: Текст для перевода.
    :return: Переведенный текст.
    """
    result = []
    # убираем символ ударения из слов
    words = source_text.split(',')
    words = [t.strip() for t in words]
    for word in words:
        word = filter_english_chars(word)
        if len(word):
            if word in dictionary:
                print('-', end='')
            else:
                dictionary[word] = smart_translate(word).lower().strip(' .')

            if word not in ['.', '-']:
                result.append(dictionary[word])

    return ', '.join(result)


def shorten(text: str) -> str:
    """
    Удаляем из текста ненужные фрагменты
    """
    text = text.replace('a black and white photo of ', '')
    text = text.replace('a black and white photograph of ', '')
    text = text.replace('an old black and white photo of ', '')
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
    tags_ru: Set[str]

    def read(self) -> None:
        """
        Считывания описания фотографии из файла.
        """
        with io.open(self.full_source, 'r', encoding='utf-8') as f:
            self.first_line_en = shorten(f.readline().strip())
            self.second_line_en = f.readline().strip()

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
    :param text: Текст для разбора. Предполагается,
    что теги отделяются друг от друга запятыми.
    :return: Список тегов.
    """
    result = [t.lower().strip() for t in text.split(',')]
    result = [t for t in result if t not in bad_tags]
    return result


def plural_if_necessary(token) -> str:
    """
    Преобразование слова во множественное число
    начальной формы при необходимости
    """
    if 'Plur' in token.morph.get('Number'):
        result = morph.parse(token.lemma_)[0].inflect({'plur'})
        if result:
            return result.word
    return token.lemma_


def get_grammemes(text: frozenset) -> set:
    """
    Получение списка граммем, позволяющих
    согласовать слова по роду и числу.
    """
    gram = {'nomn'}
    for the_one in text:
        if the_one in ['neut', 'masc', 'femn', 'plur']:
            gram.add(the_one)
    return gram


def match_word_gender(left: str, right: str) -> str:
    """
    Согласование левого слова с правым по роду и числу.
    """
    m1 = morph.parse(left)[0]
    m2 = morph.parse(right)[0]
    m3 = m1.inflect(get_grammemes(m2.tag.grammemes))
    if m3:
        return m3.word
    return left


def extract_nouns(text: str) -> List[str]:
    """
    Извлечение существительных из текста.
    :param text: Разбираемый текст.
    :return: Список существительных
    """
    doc = ru(text)
    nouns = []
    for token in doc:
        # print('->', token.text, ':', token.pos_)
        if token.pos_ in ['NOUN', 'PROPN']:
            gathered = []

            the_word = plural_if_necessary(token)

            for left in token.lefts:
                if left.pos_ == 'ADJ':
                    gathered.append(match_word_gender(str(left.lemma_), the_word))

            gathered.append(the_word)

            # for right in token.rights:
            #     if right.pos_ == 'ADJ':
            #         gathered.append(match_words(str(right.lemma_), str(token.lemma_)))

            nouns.append(' '.join(gathered))
            if len(gathered) > 1:
                nouns.append(str(token.lemma_))
    return nouns


counter = {}  # счетчик употребления слов
all_images = find_all_images()  # все найденные по указанному пути картинки

# обрабатываем найденные подписи к картинкам и сохраняем их
with io.open('tags.txt', 'w', encoding='utf-8') as output:
    for one in all_images:
        elapsed = datetime.now() - start_moment
        print(elapsed, ' => ', one.short_source, end=' ')
        one.first_line_ru = (smart_translate(one.first_line_en).lower().strip('.'))
        one_tags = get_tags(one.second_line_en)
        one.second_line_ru = fast_translate(', '.join(one_tags)).strip(' .,')
        one_nouns = extract_nouns(one.first_line_ru)
        one_items = [item.strip(' "') for item in one.second_line_ru.split(',')]
        one.tags_ru = sorted(set(one_nouns + one_items))

        for tag in one.tags_ru:
            if tag not in counter:
                counter[tag] = 0
            counter[tag] = counter[tag] + 1

        output_line = (one.full_source.removeprefix(ROOT_PATH + '\\') + '\n' +
                       one.first_line_en + '\n' +
                       one.second_line_en + '\n' +
                       one.first_line_ru + '\n' +
                       ', '.join(one.tags_ru) + '\n')
        output.write(output_line)
        output.flush()  # чтобы не потерять последние записи
        print(" OK")

# сохраняем словарь
keys = sorted(dictionary.keys())
with io.open('dictionary2.txt', 'w', encoding='utf-8') as file:
    for key in keys:
        file.write(key + '\n')
        file.write(dictionary[key] + '\n')

# сохраняем счетчик слов
keys = sorted(counter.keys())
with io.open('counter.txt', 'w', encoding='utf-8') as file:
    for key in keys:
        file.write(f"{key}\t{counter[key]}\n")
