#!/usr/bin/env python3
# -*- coding: utf-8 -*-

"""
Скрипт для извлечения метаданных ComfyUI из PNG файлов

Этот скрипт позволяет извлекать метаданные, созданные ComfyUI,
из PNG файлов, включая текстовые промпты из узлов `CLIP Text Encode (Prompt)`.

Требования:
- Python 3.6+
- Pillow (PIL)

Установка зависимостей:
pip install Pillow

Использование:
python main.py "path/to/image.png"
"""

import sys
import json
from PIL import Image

def extract_comfyui_prompt(image_path):
    try:
        img = Image.open(image_path)
        if 'prompt' not in img.info:
            print("No ComfyUI prompt metadata found in the image.")
            return

        prompt_json_str = img.info['prompt']
        workflow = json.loads(prompt_json_str)

        found = False
        for node_id, node in workflow.items():
            if node.get('class_type') == 'CLIPTextEncode':
                text = node['inputs'].get('text', '')
                if (text):
                    # print(f"Prompt from node {node_id} (CLIP Text Encode):")
                    print(text)
                    # print("\n" + "-" * 80 + "\n")
                    found = True

        if not found:
            print("No 'CLIPTextEncode' nodes found in the metadata.")
    except Exception as e:
        print(f"Error processing the image: {e}")

if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Usage: python extract_prompt.py <path_to_png_image>")
        sys.exit(1)

    extract_comfyui_prompt(sys.argv[1])
