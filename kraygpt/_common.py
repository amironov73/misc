import base64
import io
import os
import os.path


import openai
from dotenv import load_dotenv
from PIL import Image
from retry import retry


def initialize() -> None:
    global client  # type: openai.OpenAI

    load_dotenv()
    openai_api_key = os.getenv("OPENAI_API_KEY")
    openai_api_url = os.getenv("OPENAI_API_URL")
    client = openai.OpenAI(
        api_key=openai_api_key,
        base_url=openai_api_url
    )


def encode_image(input_image: str) -> str:
    """
    Кодируем картинку в BASE64.
    Если картинка слишком большая, то уменьшаем размер картинки до 1024x1024.
    :param input_image: Путь к картинке.
    :return: Содержимое картинки в BASE64.
    """

    file_size = os.stat(input_image).st_size
    if file_size > 1024 * 1024:
        image = Image.open(input_image)
        new_size = image.size
        while True:
            new_size = (new_size[0] / 2, new_size[1] / 2)
            if new_size[0] <= 1024 and new_size[1] <= 1024:
                break

        image.thumbnail(new_size, Image.Resampling.LANCZOS)
        memory = io.BytesIO()
        image.save(memory, format='jpeg')
        return base64.b64encode(memory.getvalue()).decode('utf-8')

    with open(input_image, 'rb') as image_file:
        return base64.b64encode(image_file.read()).decode('utf-8')


@retry(tries=3, delay=1, backoff=2)
def recognize_image(input_image: str, need_encode: bool = True) -> openai.ChatCompletion:
    base64_image = encode_image(input_image) if need_encode else input_image
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


def get_caption(completion: openai.Completion) -> str:
    return str(completion.choices[0].message.content).strip().replace('\n', ' ')
