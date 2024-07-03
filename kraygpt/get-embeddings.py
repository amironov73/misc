#!/usr/bin/env python3

import io
import pickle

import numpy as np
from scipy.spatial.distance import cosine

from _common import initialize, get_text_embedding

initialize()
gathered=[]
with io.open('tags-all-output.txt', 'r', encoding='utf-8') as f:
    while True:
        if len(gathered) > 1000:
            break

        f.readline()
        f.readline()
        f.readline()
        caption = f.readline().strip()
        f.readline()
        f.readline()
        f.readline()
        embedding = get_text_embedding(caption)
        gathered.append({'caption': caption, 'embedding': embedding})

with open('pickled.dat', 'wb') as f:
    pickle.dump(gathered, f)
