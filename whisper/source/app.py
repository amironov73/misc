#!/usr/bin/env python3

"""
Простой скрипт, транскрибирующий аудио.
"""

import io
import os
import sys
import torch
from transformers import AutoModelForSpeechSeq2Seq, AutoProcessor, pipeline
from datetime import datetime
from pydub import AudioSegment

def split_audio(file_path, chunk_length_ms=28000):
    audio = AudioSegment.from_file(file_path)
    chunks = []
    
    counter = 0
    for i in range(0, len(audio), chunk_length_ms):
        chunk = audio[i:i + chunk_length_ms]
        counter = counter + 1 
        chunk_file = f"chunks/chunk_{counter:05d}.wav"
        chunk.export(chunk_file, format="wav")
        print(chunk_file)
        chunks.append(chunk_file)
    
    return chunks

device = "cuda:0" if torch.cuda.is_available() else "cpu"
torch_dtype = torch.float16 if torch.cuda.is_available() else torch.float32

model_id = "openai/whisper-large-v3"

model = AutoModelForSpeechSeq2Seq.from_pretrained(
    model_id, torch_dtype=torch_dtype, low_cpu_mem_usage=True, use_safetensors=True
)
model.to(device)

processor = AutoProcessor.from_pretrained(model_id)

pipe = pipeline(
    "automatic-speech-recognition",
    model=model,
    tokenizer=processor.tokenizer,
    feature_extractor=processor.feature_extractor,
    torch_dtype=torch_dtype,
    device=device,
)


def transcribe_chunks(output, chunks):
    start_moment = datetime.now()
    
    for chunk in chunks:
        print(chunk)
        result = pipe(chunk, generate_kwargs={"language": "russian"})
        text = result['text']
        print(text)
        output.write(text)
        output.write(' ')
        output.flush()

    elapsed = datetime.now() - start_moment
    print(elapsed)

    return


if len(sys.argv) != 3:
    print("Usage: onefile.py <input_audio> <output_text>")
    exit(1)


for filename in os.listdir('chunks'):
    file_path = os.path.join('chunks', filename)
    if os.path.isfile(file_path):
        os.unlink(file_path)

chunks = split_audio(sys.argv[1])
print('=======================================')

with io.open(sys.argv[2], 'w', encoding='utf-8') as output:
    transcribe_chunks(output, chunks)

print('ALL DONE')
