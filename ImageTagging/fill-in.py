import io

dictionary = {}

# считываем словарь
with io.open('dictionary.txt', mode='r', encoding='utf-8') as f:
    for line in f:
        pair = line.strip().split(' ', maxsplit=1)
        if (len(pair) == 1):
            pair.append('')
        dictionary[pair[0]] = pair[1]

# пополняем словарь
with io.open('phase1.txt', mode='r', encoding='utf-8') as f:
    for line in f:
        key = line.strip()
        if key not in dictionary:
            dictionary[key] = ''

# записываем словарь обратно
with io.open('dictionary.txt', mode='w', encoding='utf-8') as f:
   for key in sorted(dictionary):
       value = dictionary[key]
       f.write(f"{key} {value}\n")

