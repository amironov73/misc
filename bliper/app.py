import os
from PIL import Image
from transformers import Blip2Processor, Blip2ForConditionalGeneration


ROOT_PATH = 'H:\\Images3\\FN\\000000-000099'


def caption_image(file_name: str) -> str:
    raw_image = Image.open(file_name).convert('RGB')
    inputs = processor(raw_image, question, return_tensors="pt").to("cuda:0")
    out = model.generate(**inputs, max_new_tokens=250)
    return processor.decode(out[0], skip_special_tokens=True)


# question = "Describe the image in Russian in detail."
question = "List the objects in the photo in Russian."
processor = Blip2Processor.from_pretrained("Gregor/mblip-mt0-xl")
model = Blip2ForConditionalGeneration.from_pretrained("Gregor/mblip-mt0-xl", device_map="cuda:0")

for path, _, files in os.walk(ROOT_PATH):
    for filename in files:
        ext = os.path.splitext(filename)[1].lower()
        if ext in ['.jpg']:
            image_path = os.path.join(path, filename).lower()
            caption = caption_image(image_path)
            print(filename, ' => ', caption)
