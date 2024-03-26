import io

dictionary = {}

# считываем словарь
with io.open('dictionary.txt', mode='r', encoding='utf-8') as f:
    for line in f:
        pair = line.strip().split(' ', maxsplit=1)
        if (len(pair) == 1):
            pair.append('')
        dictionary[pair[0]] = pair[1]

# считываем и переводим теги
with io.open('phase0.txt', mode='r', encoding='latin-1') as f:
    with io.open('phase2.txt', mode='w', encoding='utf-8') as o:
        for line in f:
            pair = line.strip().split('\t', maxsplit=1)
            key = pair[0].strip()
            tags = pair[1].strip().split(',')
            translated = []
            for tag in tags:
                word = tag.strip()
                if word in dictionary:
                    translated.append(dictionary[word])
            o.write(f"{key} {', '.join(translated)}\n")
