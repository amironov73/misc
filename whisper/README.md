# WHISPER portable

Простая утилита, выполняющая транскрибирование аудио и видео на русском языке с помощью нейросетевой модели `Whisper Large v3` от OpenAI.

Системные требования: Windows 10/11 x64, не менее 8 Гб RAM. Желательно наличие не менее 4-ядерного процессора.

Запуск:

```
run.cmd <input-audio> <output-text>
```

Где 

* `input-audio` - имя аудио- (или видео-) файла, подлежащего транскрибированию.

* `output-text` - имя для выходного тексового файла. ВНИМАНИЕ: файл будет перезаписан!

Пример:

```
run.cmd "D:\Events\10.11.12.mp4" sample.txt
```
