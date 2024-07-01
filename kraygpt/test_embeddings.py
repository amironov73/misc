#!/usr/bin/env python3

"""
Игрища с эмбеддингами.
"""

import numpy as np
from scipy.spatial.distance import cosine

from _common import initialize, get_text_embedding


def test_embedding(text: str) -> None:
    """
    Тестирование эмбеддинга текста.
    """
    embedding = get_text_embedding(text)
    print(text)
    print(embedding)


initialize()

embedding1 = np.array (get_text_embedding("Hello, world!"))
embedding2 = np.array (get_text_embedding("Привет, мир!"))
embedding3 = np.array (get_text_embedding("На фото изображена городская улица с трамваями и автобусами. Люди идут по улице, вокруг зима — лежит снег. На заднем плане видны здания, а на переднем — часть ограды и столб."))
embedding4 = np.array (get_text_embedding("На фото изображён городской пейзаж: улица с проходящим трамваем, прохожие, здание с балконами и телега с лошадью. Сцена выглядит старомодно, возможно, советских времён."))

first_difference = cosine(embedding1, embedding2)
print(first_difference)

second_difference = cosine(embedding3, embedding4)
print(second_difference)
