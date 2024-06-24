#!/usr/bin/env python3

"""
Простая программа для распознания изображений
на русском языке с помощью GPT-4o.
"""

import os
import sys

from dotenv import load_dotenv
from openai import OpenAI

from _common import recognize_image

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
