import io
from googletrans import Translator
from time import sleep

translator = Translator()
dictionary = {}

with io.open('dictionary.txt', mode='r', encoding='utf-8') as file:
    for line in file:
        pair = line.strip().split(' ', maxsplit=1)
        if len(pair) == 2:
            dictionary[pair[0]] = pair[1]
        else:
            translation = translator.translate(pair[0], src='en', dest='ru').text
            dictionary[pair[0]] = translation
            print(pair[0], '->', translation)
            sleep(0.05)

with io.open('dictionary.txt', mode='w', encoding='utf-8') as file:
    for key in sorted(dictionary):
        value = dictionary[key]
        file.write(f"{key} {value}\n")
