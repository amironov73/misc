#!/usr/bin/env python3

import io
import pickle

import numpy as np
from scipy.spatial.distance import cosine

from _common import initialize, get_text_embedding

initialize()

with open('pickled.dat', 'rb') as f:
    database = pickle.load(f)

question = 'медали'
embedding = np.array(get_text_embedding(question))

result=[]
for one in database:
    applicant = np.array(one['embedding'])
    d = cosine(embedding, applicant)
    result.append({'caption': one['caption'], 'delta': d})

result=sorted(result, key=lambda item: item['delta'])

for one in result[:5]:
    print(one['delta'], ' => ', one['caption'])
