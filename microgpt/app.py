from microgpt import MicroGPT

if __name__ == '__main__':
    gpt = MicroGPT('http://192.168.7.50:1234')

    # Выясняем id используемой модели
    models = gpt.models()
    print(models[0]["id"])

    # Переводим текст
    system_prompt = "You literally translate text from English into Russian."
    user_prompt = "Дословно переведи на русский язык: two men near the ice-covered tree"
    temperature = 0.1
    completions = gpt.completions(user_prompt, system_prompt, temperature)
    for choice in completions["choices"]:
        print(choice["message"]["content"])
