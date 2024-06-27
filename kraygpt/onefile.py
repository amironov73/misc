#!/usr/bin/env python3

"""
Простая программа для распознания изображений
на русском языке с помощью GPT-4o.
"""

import sys

from _common import initialize, recognize_image


if len(sys.argv) != 2:
    print("Usage: onefile.py <input_image>")
    exit(1)

initialize()
completion = recognize_image(sys.argv[1])
print(completion.choices[0].message.content)
