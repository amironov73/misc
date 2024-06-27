#!/usr/bin/env python3

"""
Окно, на которое можно кидать картинки,
чтобы посмотреть, как они распознаются.
"""

import io
import base64

import gradio as gr

from _common import initialize, recognize_image, get_caption


def handle_image(image):
    # Конвертируем изображение в base64
    buffered = io.BytesIO()
    image.save(buffered, format="JPEG")
    img_str = base64.b64encode(buffered.getvalue()).decode("utf-8")
    response = recognize_image(img_str, False)

    # Извлекаем результат распознавания
    return get_caption(response)


initialize()

# Создаем интерфейс Gradio
interface = gr.Interface(
    fn=handle_image,
    inputs=gr.Image(type="pil"),
    outputs=gr.Textbox(label="Сгенерированное описание изображения",
                       show_copy_button=True,
                       lines=15),
    allow_flagging="never",
    submit_btn=gr.Button("Отправить", variant="primary"),
    clear_btn=gr.Button("Очистить", variant="secondary"),
    title="Распознавание изображений с помощью GPT-4o",
    description="Загрузите изображение, которое вы хотите распознать"
)

# Запускаем приложение
if __name__ == "__main__":
    interface.launch(inbrowser=True)
