#!/usr/bin/env python3

import io
import spacy
import pymorphy2 as pm
from typing import List

ru = spacy.load("ru_core_news_lg")
morph = pm.MorphAnalyzer()

def get_gram(text: frozenset) -> set:
    gram = set()
    for one in text:
        if one in ['neut', 'masc', 'femn']:
            gram.add(one)
    return gram


def match_words(left: str, right: str) -> str:
    m1 = morph.parse(left)[0]
    m2 = morph.parse(right)[0]
    m3 = m1.inflect(get_gram(m2.tag.grammemes))
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

            for left in token.lefts:
                if left.pos_ == 'ADJ':
                    gathered.append(match_words(str(left.lemma_), str(token.lemma_)))

            gathered.append(str(token.lemma_))

            # for right in token.rights:
            #     if right.pos_ == 'ADJ':
            #         gathered.append(match_words(str(right.lemma_), str(token.lemma_)))

            nouns.append(' '.join(gathered))
            if len(gathered) > 1:
                nouns.append(str (token.lemma_))
    return nouns


with io.open("input.txt", "r", encoding="utf8") as f:
    for line in f:
        line = line.strip()
        print (line)
        nouns = extract_nouns(line)
        for noun in nouns:
            print('*', noun)

