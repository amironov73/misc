#!/usr/bin/env python3

"""
Простая программа для распознания изображений
на русском языке с помощью GPT-4o.
"""

import os
import sys
import base64

import openai
from dotenv import load_dotenv
from openai import OpenAI


def encode_image(input_image: str) -> str:
    """
    Кодируем картинку в BASE64.
    :param input_image: Путь к картинке.
    :return: Содержимое картинки в BASE64.
    """
    with open(input_image, "rb") as image_file:
        return base64.b64encode(image_file.read()).decode('utf-8')


def recognize_image(input_image: str) -> 'openai.ChatCompletion':
    base64_image = encode_image(input_image)
    return client.chat.completions.create(
        model="gpt-4o",
        max_tokens=100,
        messages=[
            {"role": "user",
             "content":
                 [
                     {"type": "text", "text": "Очень кратко опиши фото"},
                     {"type": "image_url",
                      "image_url":
                          {
                              "detail": "low",
                              "url": f"data:image/jpeg;base64,{base64_image}"
                          }
                      }
                 ]
             }
        ]
    )


load_dotenv()

OPENAI_API_KEY = os.getenv("OPENAI_API_KEY")
OPENAI_API_URL = os.getenv("OPENAI_API_URL")
client = OpenAI(
    api_key=OPENAI_API_KEY,
    base_url=OPENAI_API_URL
)

if len(sys.argv) != 2:
    print("Usage: onefile.py <input_image>")
    exit(1)

completion = recognize_image(sys.argv[1])
print(completion.choices[0].message.content)
