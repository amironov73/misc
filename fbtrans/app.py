from transformers import AutoTokenizer, AutoModelForSeq2SeqLM, pipeline

# checkpoint = 'facebook/nllb-200-distilled-600M'
# checkpoint = 'facebook/nllb-200–1.3B'
checkpoint = 'facebook/nllb-200-3.3B'
# checkpoint = 'facebook/nllb-200-distilled-1.3B'

model = AutoModelForSeq2SeqLM.from_pretrained(checkpoint)
tokenizer = AutoTokenizer.from_pretrained(checkpoint)
translator = pipeline('translation', model=model, tokenizer=tokenizer,
                      src_lang='eng_Latn', tgt_lang='rus_Cyrl', max_length = 400)
source_text = 'two men near the ice-covered tree'
output = translator(source_text)
translated_text = output[0]['translation_text']
print(translated_text)