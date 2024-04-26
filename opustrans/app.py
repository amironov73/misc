from transformers import MarianMTModel, MarianTokenizer

device = 'cuda' #or 'cpu' for translate on cpu

model_name = 'Helsinki-NLP/opus-mt-en-ru'
tokenizer = MarianTokenizer.from_pretrained(model_name)
model = MarianMTModel.from_pretrained(model_name)
# model.to(device)

src_text = ['two men near the ice-covered tree']
translated = model.generate(**tokenizer(src_text, return_tensors="pt", padding=True))
result = [tokenizer.decode(t, skip_special_tokens=True) for t in translated]
print(result[0])

