#!/usr/bin/env python3

"""
Простая программа, конвертирующая текстовый файл в аудио
с применением SaluteSpeech.
"""

import os
import spacy
import io
import requests
import uuid
import time
import wave
import argparse
import re

import urllib3
from dotenv import load_dotenv

# Максимальный размер тела запроса — 4 000 символов, включая пробелы и SSML-разметку.
# Делаем скидку на возможное добавление служебных тегов
MAX_PACKET_SIZE = 3950
VOICE = 'Tur_24000'
OUT_DIRECTORY = 'out'
OUT_FILENAME = 'book.wav'

# Таблица соответствия кратких обозначений голосов
VOICE_TABLE = {
    'N': 'Nec_24000',
    'B': 'Bys_24000',
    'M': 'May_24000',
    'T': 'Tur_24000',
    'O': 'Ost_24000',
    'P': 'Pon_24000'
}

chunk_number = 0
accumulated_text = ''

# Разбор аргументов командной строки
parser = argparse.ArgumentParser(description='SaluteBook')
parser.add_argument('input', nargs=1, type=str,
                    help='Input file name')
parser.add_argument('--voice', type=str, default=VOICE,
                    help='Name of the voice')
parser.add_argument('--dir', type=str, default=OUT_DIRECTORY,
                    help='Directory to save the output')
parser.add_argument('--output', type=str, default=OUT_FILENAME,
                    help='Name of file to save the output')
args = parser.parse_args()
INPUT_FILE = args.input[0]
VOICE = args.voice
OUT_DIRECTORY = args.dir
OUT_FILENAME = args.output
access_token = ''


def retrieve_access_token() -> None:
    global access_token
    access_token = requests.post('https://ngw.devices.sberbank.ru:9443/api/v2/oauth',
                                 verify=False,
                                 data='scope=SALUTE_SPEECH_PERS',
                                 headers={'Authorization': f'Basic {auth_data}',
                                          'RqUID': request_id,
                                          'Content-Type': 'application/x-www-form-urlencoded'}).json()['access_token']


def expand_voice(obj: re.Match) -> str:
    """Расширение краткого обозначения голоса"""
    voice = obj.group(1)
    voice = VOICE_TABLE.get(voice, voice)
    return f'<voice name="{voice}">{obj.group(2)}</voice>'


def generate_audio(text: str) -> bytes:
    """
    Генерация аудио по тексту с помощью сервиса.
    :param text: Озвучиваемый текст.
    :return: Содержимое WAV-файла.
    """
    body = text
    if "<speak>" not in text:
        body = f"<speak>{text}</speak>"
    while True:
        answer = requests.post(f"https://smartspeech.sber.ru/rest/v1/text:synthesize?format=wav16&voice={VOICE}",
                               verify=False,
                               headers={'Authorization': f'Bearer {access_token}',
                                        'Content-Type': 'application/ssml'},
                               data=body)

        if answer.status_code == 200:
            return answer.content

        if answer.status_code == 401:
            retrieve_access_token()
        else:
            print(f"Error: {answer}")
            exit(1)


def flush_accumulated() -> None:
    """
    Отправка накопленного текста сервису на озвучивание.
    """
    global accumulated_text, chunk_number
    if not accumulated_text:
        return

    print(f"=> Chunk {chunk_number:04}", end=': ')
    start = time.time()
    audio = generate_audio(accumulated_text)
    end = time.time()
    chunk_path = os.path.join(OUT_DIRECTORY, f'chunk{chunk_number:04}.wav')
    with open(chunk_path, 'wb') as o:
        o.write(audio)
    print(f"{end - start:.2f} seconds")
    accumulated_text = ''
    chunk_number += 1


def append_chunk(text: str) -> None:
    """
    Добавление очередного фрагмента текста.
    :param text: Озвучиваемый текст.
    """
    global accumulated_text
    candidate = accumulated_text + text
    body_length = len(candidate.encode('utf-8'))
    if body_length > MAX_PACKET_SIZE:
        flush_accumulated()
        candidate = text
    accumulated_text = candidate


def merge_chunks() -> None:
    """
    Сшивание озвученных фрагментов в общий файл аудиокниги.
    """
    print('Merging chunks...', end=' ')
    merged = []
    for file in os.listdir(OUT_DIRECTORY):
        w = wave.open(f'{OUT_DIRECTORY}/{file}', 'rb')
        merged.append([w.getparams(), w.readframes(w.getnframes())])
        w.close()

    output_path = os.path.join(OUT_DIRECTORY, OUT_FILENAME)
    output = wave.open(output_path, 'wb')
    output.setparams(merged[0][0])
    for i in range(len(merged)):
        output.writeframes(merged[i][1])
    output.close()
    print('DONE')


def expand_markup(text: str) -> str:
    """
    Раскрытие разметки, например, пауз.
    :param text: Текст с разметкой.
    :return: Раскрытый текст.
    """
    text = re.sub(r'\{\.}', '<break />', text) # пауза
    text = re.sub(r'\{@(\w+?)}(.+?){/}', expand_voice, text) # голос
    text = re.sub(r'\{p(\w+?)}(.+?){/}', # тон
                    r'<paint pitch="\1">\2</paint>', text)
    text = re.sub(r'\{s(\w+?)}(.+?){/}', # скорость
                    r'<paint speed="\1">\2</paint>', text)
    text = re.sub(r'\{l(\w+?)}(.+?){/}', # громкость
                    r'<paint loudness="\1">\2</paint>', text)
    text = re.sub(r'\{b(\w+?)}(.+?){/}', # фоновые звуки
                    r'<extra.background-audio src="\1">\2</extra.background-audio>',
                    text)
    return text


# Создаем папку для результата (если она не существует)
try:
    os.mkdir(OUT_DIRECTORY)
except:
    pass

# Удаляем все имеющиеся файлы
for (_, _, filenames) in os.walk(OUT_DIRECTORY):
    for filename in filenames:
        full_filename = os.path.join(OUT_DIRECTORY, filename)
        os.remove(full_filename)

# Запоминаем момент начала обработки
start_time = time.time()

ru = spacy.load('ru_core_news_lg')
ru.add_pipe('sentencizer')

load_dotenv()
auth_data = os.getenv('AUTH_DATA')
if not auth_data:
    print('AUTH_DATA not set')
    exit(1)

request_id = str(uuid.uuid4())

urllib3.disable_warnings()
retrieve_access_token()

# Построчно считываем файл.
# Считаем каждую строку отдельным абзацем.
with io.open(INPUT_FILE, 'r', encoding='utf-8') as f:
    while True:
        line = f.readline()
        if not line:
            break

        line = line.strip()
        if not line:
            # Пустые строки игнорируем.
            continue

        # Разбиваем каждый абзац на предложения
        doc = ru(line)
        sentences = [sent.text.strip() for sent in doc.sents]
        for sentence in sentences:
            print(sentence[:40])
            expanded = expand_markup(sentence)
            # Добавляем короткую паузу между предложениями.
            append_chunk(expanded + '<break time="200ms" />')

        # Добавляем короткую паузу между абзацами.
        append_chunk('<break time="200ms" />')

        print()

flush_accumulated()
merge_chunks()

# Выводим общее затраченное время
finish_time = time.time()
print(f"Total elapsed time: {finish_time - start_time:.2f} seconds")
