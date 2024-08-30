# SaluteBook

Простая утилита для конвертации текстового файла в аудиокнигу при помощи API SaluteSpeech.

Утилита требует наличия файла `.env` следующего формата:

```
AUTH_DATA=аутентификационные данные для сервиса SaluteSpeech.
```

Командная строка:

```
python [-u] app.py <input> [--voice <voice>]
```

здесь

* `<input>`  - обязательное имя текстового файла, подлежащего озвучке. Файл должен содержать текст на русском языке в кодировке UTF-8.

* `-u` - необязательный ключ, указывающий интерпретатору применять небуферизованный консольный ввод-вывод.

* `--voice <voice>` - использование голоса (см. идентификаторы голосов ниже).

Пример запуска:

```
pyton app.py katya.txt
```

здесь `katya.txt` - файл, содержащий историю Кати Гориной. При успешном завершении работы скрипта результат - файл `book.wav` будет помещен в папку `out`, которую при необходимости скрипт создает самостоятельно. Содержимое этой папки перед началом работы скрипта очищается автоматически, позаботьтесь о том, чтобы переместить предыдущий результат работы в нужную папку.

### Доступные голоса

* Наталья `Nec_24000` (кратко `N`)
* Борис `Bys_24000` (кратко `B`)
* Марфа `May_24000` (кратко `M`)
* Тарас `Tur_24000` (кратко `T`)
* Александра `Ost_24000` (кратко `O`)
* Сергей `Pon_24000` (кратко `P`)
* Kira `Kin_24000` - синтез английской речи

## Формат файла

Озвучиваемый файл может содержать базовую HTML-разметку, используемую, например, в формате FictionBook:

```html
<p>Светлана Петровна вызвала меня неожиданно. А я был не из тех, кого по английскому языку можно вызывать неожиданно. По математике, физике или химии – пожалуйста, но по английскому – ни в коем случае. Железная тройка, полученная на прошлой неделе, вроде бы обеспечивала мне полмесячную передышку, и вот тебе!</p>

<p>Я хотел было отказаться сразу, но Август Шулин, мой сосед, испуганно вытолкнул меня из-за стола, и я, как порядочный пошел к доске, кивками прося подсказывать. И сразу кто-то зашипел, рупором прижав ладони ко рту или в свернутую трубкой тетрадку, кто-то беззвучно корчил рожи, надеясь, что я все прочту по губам.</p>

<p>Светлана Петровна терпела, терпела, потом устало вздохнула и сказала по-русски:</p>

<p>- Ну, хватит. Бесполезны ваши старания. Кажется, дня три не открывали учебника, так ведь, Эпов?</p>
```

Прочие HTML-теги, например, `<h1>`, использовать нельзя - сервис вернет ошибку.

Учтите, что символы HTML-разметки учитываются сервисом при списании средств, при том что в аудио-файл не попадают.

В озвучиваемом файле можно использовать разметку SSML - Speech Synthesis Markup Language.

### Альтернативный голос

См

```xml
<speak>
    Добрый день!
    <voice name="Bys_24000">Здравствуйте!</voice>
</speak>
```

Кратко то же самое можно записать так:

```text
Добрый день!
{@B}Здравствуйте!{/}
```

### Пауза

Можно задать длительность паузы явно.

```xml
<break time="200ms" />
```

Можно задать длительность паузы в виде именованного интервала.

```xml
<break strength="weak" />
```

Здесь имя интервала может принимать следующие значения:

* x-weak;
* weak;
* medium;
* strong;
* x-strong.

Если задать просто `<break />` без атрибутов, то по умолчанию это будет эквивалентно

```xml
<break strength="medium" />
```

Кратко паузу можно записать так:

```text
Немного помолчал{.} и продолжил.
```

### Исправление произношения

Чтобы исправить произношение на более привычное, надо написать.

```xml
<sub alias="TEXT">REPLACED_TEXT</sub>
```

здесь

* `TEXT` — текст для генерации.
* `REPLACED_TEXT` — заменяемый текст.
* 

Пример:

```xml
<speak>Дни и ночи у <sub alias= "мартэновских">
    мартеновских</sub> печей</speak>
```

### Аббревиатуры, даты и числительные

Тег `<say-as>` включает в себя атрибут `interpret-as`. Значение этого атрибута определяет дальнейшее произнесение синтезируемого текста.

* `cardinal` - количественное числительное

```xml
<speak>
    <say-as interpret-as="cardinal" format="feminine_nominative">
        1
    </say-as> 
    пачка
</speak>
```

См. также `ordinal`.

* `ordinal` - порядковое числительное

```xml
<speak>
    <say-as interpret-as="ordinal" format="feminine_nominative">
        1
    </say-as>
    пачка
</speak>
```

Значения cardinal и ordinal атрибута interpret-as отвечают за произнесение количественного и порядкового числительных соответственно.

```xml
<say-as interpret-as="VALUE" format="GENDER_CASE">123</say-as>
```

Значения атрибута `format` определяют, какой род и падеж использовать для синтезирования текста. Строковый параметр `GENDER` принимает следующие значения:

* `masculine` — мужской род;
* `feminine` — женский род;
* `neuter` — средний род;
* `plural` — множественное число.

Строковый параметр `CASE` принимает следующие значения:

* `nominative` — именительный;
* `genitive` — родительный;
* `dative` — дательный;
* `accusative` — винительный для одушевленных;
* `accusative_dead` — винительный для неодушевленных;
* `ablative` — творительный;
* `prepositional` — предложный.

По умолчанию атрибут `format` принимает значения `GENDER = masculine` и `CASE = nominative`. Также атрибут `format` можно указывать в формате `CASE_GENDER` или `GENDER_CASE`.

В качестве значений тега `<say-as>` можно использовать отрицательные числа и начинать числа с нуля. Для обоих видов числительных поддерживаются числа не выше миллиардов. Если для `cardinal` указать число больше, в ответе будет количество миллиардов. Например: `1 000 000 000 000 — «тысяча миллиардов»`. Если для `ordinal` указать больше, то в ответ придет ошибка.

Пример произношения числа `1` в женском роде родительном падеже — «одной»:

```xml
<speak>
    <say-as interpret-as="cardinal" format="feminine_genitive">
        1
    </say-as>
</speak>
```

* `spell-out` - произнесение аббревиатур более привычным способом

```xml
<speak><say-as interpret-as="spell-out">МКС</say-as></speak>
```

* `telephone` - номер мобильного телефона   

```xml
<speak>
    <say-as interpret-as="telephone" detail="VAL_VAL">
        +7 (909) 2282424
    </say-as>
</speak>
```

* `money` - денежная сумма

```xml
<speak>
    <say-as interpret-as="money" detail="USD">
        21
    </say-as>
</speak>
```

Значение `money` атрибута `interpret-as` отвечает за произнесение денежных сумм в заданной валюте.

```xml
<say-as interpret-as="money" format="CASE" detail="CURRENCY_short-form_say-null-cents">VALUE</say-as>
```

Атрибут `format` имеет строковый параметр `CASE` с указанием падежа:

* `nominative` — именительный;
* `genitive` — родительный;
* `dative` — дательный;
* `accusative` — винительный;
* `ablative` — творительный;
* `prepositional` — предложный.

Параметры в атрибуте `detail` необходимо указывать через знак `_`.

Строковый параметр `CURRENCY` указывает валюту (по умолчанию используется рубль). Доступные варианты:

* `RUB` — рубль;
* `USD` — доллар США;
* `EUR` — евро;
* `GBP` — фунт стерлингов;
* `CAD` — канадский доллар;
* `CHF` — швейцарский франк;
* `SEK` — шведская крона;
* `DKK` — датская крона;
* `NOK` — норвежская крона;
* `JPY` — японская йена;
* `CNY` — китайский юань;
* `PLN` — польский злотый.

Можно дополнить произнесение денежных сумм следующими вариантами:

* `full-form` — полные формы названий валют. Например, «японская йена».
* `short-form` — короткие формы названий валют. Например, «доллар» вместо «доллар США».
* `say-null-cents` — «центы» будут произнесены в любом случае, даже если их ноль. Например, «десять долларов, ноль центов».

Примеры:

```xml
<speak>
    <say-as interpret-as="money" format="genitive" detail="say-null-cents">
        10
    </say-as>
</speak>
```

Результат: `десяти рублей, ноля копеек`.

```xml
<speak>
    <say-as interpret-as="money" detail="USD">
        21
    </say-as>
</speak>
```

Результат: `двадцать один доллар США`.

```xml
<speak>
    <say-as interpret-as="money" detail="USD_short-form">
        21,15
    </say-as>
</speak>
```

Результат: `двадцать один доллар, пятнадцать центов`.

* `date` - дата

```xml
<speak>
    <say-as interpret-as="date" detail="d.m.y">
        25.01.2000
    </say-as>
</speak>
```

Значение `date` атрибута `interpret-as` отвечает за произнесение дат.

```xml
<say-as interpret-as="date" format="CASE" detail="TEMPLATE">
    VALUE
</say-as>
```

Атрибут `format` имеет строковый параметр `CASE` с указанием падежа:

* `nominative` — именительный;
* `genitive` — родительный;
* `dative` — дательный;
* `accusative` — винительный для одушевленных;
* `accusative_dead` — винительный для неодушевленных;
* `ablative` — творительный;
* `prepositional` — предложный.

* Параметр `TEMPLATE` атрибута `detail` задает порядок, в котором указаны число, месяц и год. Можно указать только два или только одну часть, например, число и месяц без года или только год. Допускаются только символы, указанные ниже, которые должны иметь любой из разделителей: `.`, `-` или `/`.

* `d` — шаблон для числа месяца;
* `m` — шаблон для месяца;
* `y` — шаблон для года;
* `yw` (вместо `y`) — в этом случае будет опущено слово год («двухтысячный» вместо «двух тысячный год»).

Дата `VALUE` представляет собой числа с одним из разделителей: `.`, `-` или `/`. Порядок и наличие числа, месяца и года должен соответствовать `TEMPLATE`. Если `TEMPLATE` не указан, используется по умолчанию формат даты `d.m.y`. День и месяц даты можно указывать одной или двумя цифрами. Год можно указывать от одной до четырех цифр.

Примеры:

```xml
<speak>
    <say-as interpret-as="date" detail="d.m.y">
        25.01.2000
    </say-as>
</speak>
```

Результат: `двадцать пятое января двухтысячного года`.

```xml
<speak>
    <say-as interpret-as="date" detail="m.yw">
        01.2000
    </say-as>
</speak>
```

Результат: `январь двухтысячного`.

```xml
<speak>
    <say-as interpret-as="date" format="accusative" detail="y">
        2000
    </say-as>
</speak>
```

Результат: `двухтысячный год`.

### Громкость, скорость, тон

Тег `<paint>` используется для указания характеристик, с которыми нужно прочитать текст.

У тега есть несколько атрибутов:

* `pitch` - тон голоса

```xml
<speak>
    <paint pitch="value">
        текст
    </paint>
</speak>
```

кратко:

```text
Изменяем {p1}тон голоса{/}.
```

* `slope` - интонация: нисходящая, ровная, восходящая

```xml
<speak>
    <paint slope="value">
        текст
    </paint>
</speak>
```

* `speed` - скорость

```xml
<speak>
    <paint speed="value">
        текст
    </paint>
</speak>
```

Кратко:

```text
Изменяем {s5}скорость{/}.
```

* `loudness` - громкость

```xml
<speak>
    <paint loudness="value">
        текст
    </paint>
</speak>
```

`value` может быть цифрой от `1` до `5` или принимать значение `default`. Кроме того, вы можете использовать значения `x-low`, `low`, `medium`, `high` и `x-high`.

Поддерживаются вложенные и множественные теги, например:

```xml
<speak>
    <paint pitch="5" speed="4">
        высоко и быстро,
        
        <paint loudness="5">
            а еще и громко
        </paint>
    </paint>
</speak>
```

Вот как можно сделать в вопросе акцент на *песиках* (именно на песиках):

```xml
<speak>
    <paint pitch="3" slope="1">
        любишь
    </paint>
    
    <paint pitch="5" slope="5" loudness="4">
        песиков?
    </paint>
    
</speak>
```

### Выделение слова интонацией

Чтобы с помощью интонации выделить конкретное слово в речи, используйте символ `*`. Звездочка ставится вплотную к слову.

```xml
<speak>Сахара — самая *большая пустыня</speak>
```

### Ударения в словах

По умолчанию ассистент расставляет ударения в словах с учетом контекста. Но иногда для корректного произношения необходимо проставить ударения вручную. Для этого после ударной буквы добавьте апостроф — `'`.

Пример с ударением на А:

```xml
<speak>за'мок</speak>
```

Пример с ударением на О:

```xml
<speak>замо'к</speak> 
```

### Обработка букв Е и Ё

По умолчанию речь синтезируется с учетом контекста. Слова с буквой «ё» произносятся правильно независимо от того, как они написаны. Например, слово «ёлка» будет звучать правильно, даже если буква «ё» не указана.

Для принудительного использования «ё» просто пишите ее там, где она нужна. Если в слове буква «е» произносится ошибочно как «ё», тогда поставьте ударение в слове на ударный слог с буквой «е», используя апостроф — `'`. Обратите внимание, что апостроф ставится после ударной буквы.

Пример с ударением:

```xml
<speak>бере'т</speak>
```

### Фоновые звуки

Можно добавить фоновый звук в синтезированную речь.

```xml
<speak><extra.background-audio src="sm-sounds-nature-rain-2">
    Ну и погодка! Добрый хозяин собаку из дома не выгонит 
</extra.background-audio>
</speak>
```

В стандартную библиотеку звуков входят:

**Люди**

* Аплодисменты `sm-sounds-human-cheer-1`
* Болельщики `sm-sounds-human-cheer-2`
* Дети `sm-sounds-human-kids-1`
* Зомби №1 (рык) `sm-sounds-human-walking-dead-1`
* Зомби №2 (стон) `sm-sounds-human-walking-dead-2`
* Зомби №3 (рык с криком) `sm-sounds-human-walking-dead-3`
* Кашель №1 `sm-sounds-human-cough-1`
* Кашель №2 `sm-sounds-human-cough-2`
* Смех №1 `sm-sounds-human-laugh-1`
* Смех №2 `sm-sounds-human-laugh-2`
* Смех №3 (злодейский) `sm-sounds-human-laugh-3`
* Смех №4 (злодейский) `sm-sounds-human-laugh-4`
* Смех №5 (детский) `sm-sounds-human-laugh-5`
* Толпа №1 (разговоры) `sm-sounds-human-crowd-1`
* Толпа №2 (удивление) `sm-sounds-human-crowd-2`
* Толпа №3 (аплодисменты) `sm-sounds-human-crowd-3`
* Толпа №4 (бурные аплодисменты) `sm-sounds-human-crowd-4`
* Толпа №5 (болельщики) `sm-sounds-human-crowd-5`
* Толпа №6 (одобрительные крики) `sm-sounds-human-crowd-6`
* Толпа №7 (недовольство) `sm-sounds-human-crowd-7`
* Чихание №1 `sm-sounds-human-sneeze-1`
* Чихание №2 `sm-sounds-human-sneeze-2`
* Шаги в комнате `sm-sounds-human-walking-room-1`
* Шаги на снегу `sm-sounds-human-walking-snow-1`
* Шаги по листьям `sm-sounds-human-walking-leaves-1`

**Животные**

* Волк `sm-sounds-animals-wolf-1`
* Ворона №1 `sm-sounds-animals-crow-1`
* Ворона №2 `sm-sounds-animals-crow-2`
* Корова №1 `sm-sounds-animals-cow-1`
* Корова №2 `sm-sounds-animals-cow-2`
* Корова №3 `sm-sounds-animals-cow-3`
* Кошка №1 (мяуканье) `sm-sounds-animals-cat-1`
* Кошка №2 (мяуканье) `sm-sounds-animals-cat-2`
* Кошка №3 (мяуканье) `sm-sounds-animals-cat-3`
* Кошка №4 (мурчание) `sm-sounds-animals-cat-4`
* Кошка №5 (шипение) `sm-sounds-animals-cat-5`
* Кукушка `sm-sounds-animals-cuckoo-1`
* Курица `sm-sounds-animals-chicken-1`
* Лев №1 `sm-sounds-animals-lion-1`
* Лев №2 `sm-sounds-animals-lion-2`
* Лошадь №1 (ржание) `sm-sounds-animals-horse-1`
* Лошадь №2 (фырканье) `sm-sounds-animals-horse-2`
* Лошадь №3 (галоп) `sm-sounds-animals-horse-galloping-1`
* Лошадь №4 (шаг) `sm-sounds-animals-horse-walking-1`
* Лягушка `sm-sounds-animals-frog-1`
* Морская чайка `sm-sounds-animals-seagull-1`
* Обезьяна `sm-sounds-animals-monkey-1`
* Овца №1 `sm-sounds-animals-sheep-1`
* Овца №2 (несколько) `sm-sounds-animals-sheep-2`
* Петух `sm-sounds-animals-rooster-1`
* Слон №1 `sm-sounds-animals-elephant-1`
* Слон №2 `sm-sounds-animals-elephant-2`
* Собака №1 (лай) `sm-sounds-animals-dog-1`
* Собака №2 (рык) `sm-sounds-animals-dog-2`
* Собака №3 (скуление) `sm-sounds-animals-dog-3`
* Собака №4 (лай) `sm-sounds-animals-dog-4`
* Собака №5 (лай) `sm-sounds-animals-dog-5`
* Сова №1 `sm-sounds-animals-owl-1`
* Сова №2 `sm-sounds-animals-owl-2`

**Природа**

* Ветер №1 `sm-sounds-nature-wind-1`
* Ветер №2 `sm-sounds-nature-wind-2`
* Гром №1 `sm-sounds-nature-thunder-1`
* Гром №2 `sm-sounds-nature-thunder-2`
* Джунгли №1 `sm-sounds-nature-jungle-1`
* Джунгли №2 `sm-sounds-nature-jungle-2`
* Дождь №1 `sm-sounds-nature-rain-1`
* Дождь №2 `sm-sounds-nature-rain-2`
* Лес №1 `sm-sounds-nature-forest-1`
* Лес №2 `sm-sounds-nature-forest-2`
* Море №1 `sm-sounds-nature-sea-1`
* Море №2 `sm-sounds-nature-sea-2`
* Огонь №1 `sm-sounds-nature-fire-1`
* Огонь №2 `sm-sounds-nature-fire-2`
* Ручей №1 `sm-sounds-nature-stream-1`
* Ручей №2 `sm-sounds-nature-stream-2`

**Музыка**

* Арфа `sm-music-harp-1`
* Барабанный проигрыш №1 `sm-music-drums-1`
* Барабанный проигрыш №2 `sm-music-drums-2`
* Барабанный проигрыш №3 `sm-music-drums-3`
* Бит №1 (быстро) `sm-music-drum-loop-1`
* Бит №2 (медленно) `sm-music-drum-loop-2`
* Бубен №1 (80 ударов в минуту) `sm-music-tambourine-80bpm-1`
* Бубен №2 (100 ударов в минуту) `sm-music-tambourine-100bpm-1`
* Бубен №3 (120 ударов в минуту) `sm-music-tambourine-120bpm-1`
* Волынка №1 `sm-music-bagpipes-1`
* Волынка №2 `sm-music-bagpipes-2`
* Гитара, аккорд C `sm-music-guitar-c-1`
* Гитара, аккорд E `sm-music-guitar-e-1`
* Гитара, аккорд G `sm-music-guitar-g-1`
* Гитара, аккорд А `sm-music-guitar-a-1`
* Гонг №1 `sm-music-gong-1`
* Гонг №2 `sm-music-gong-2`
* Горн `sm-music-horn-2`
* Скрипка (до) `sm-music-violin-c-1`
* Скрипка (до верхнее) `sm-music-violin-c-2`
* Скрипка (ля) `sm-music-violin-a-1`
* Скрипка (ми) `sm-music-violin-e-1`
* Скрипка (ре) `sm-music-violin-d-1`
* Скрипка (си) `sm-music-violin-b-1`
* Скрипка (соль) `sm-music-violin-g-1`
* Скрипка (фа) `sm-music-violin-f-1`
* Труба болельщика `sm-music-horn-1`
* Фортепиано (до) `sm-music-piano-c-1`
* Фортепиано (до верхнее) `sm-music-piano-c-2`
* Фортепиано (ля) `sm-music-piano-a-1`
* Фортепиано (ми) `sm-music-piano-e-1`
* Фортепиано (ре) `sm-music-piano-d-1`
* Фортепиано (си) `sm-music-piano-b-1`
* Фортепиано (соль) `sm-music-piano-g-1`
* Фортепиано (фа) `sm-music-piano-f-1`

**Игры**

* Загрузка (8 бит) `sm-sounds-game-boot-1`
* Монета (8 бит) №1 `sm-sounds-game-8-bit-coin-1`
* Монета (8 бит) №2 `sm-sounds-game-8-bit-coin-2`
* Неудача №1 `sm-sounds-game-loss-1`
* Неудача №2 `sm-sounds-game-loss-2`
* Неудача №3 `sm-sounds-game-loss-3`
* Оповещение (8 бит) `sm-sounds-game-ping-1`
* Победные фанфары №1 `sm-sounds-game-win-1`
* Победные фанфары №2 `sm-sounds-game-win-2`
* Победные фанфары №3 `sm-sounds-game-win-3`
* Полет (8 бит) `sm-sounds-game-8-bit-flyby-1`
* Пулемет (8 бит) `sm-sounds-game-8-bit-machine-gun-1`
* Телефон (8 бит) `sm-sounds-game-8-bit-phone-1`
* Усиление (8 бит) №1 `sm-sounds-game-powerup-1`
* Усиление (8 бит) №2`sm-sounds-game-powerup-2`

**Вещи**

* Бензопила `sm-sounds-things-chainsaw-1`
* Взрыв `sm-sounds-things-explosion-1`
* Вода (наливается в стакан) `sm-sounds-things-water-3`
* Вода №1 (льется) `sm-sounds-things-water-1`
* Вода №2 (бурлит) `sm-sounds-things-water-2`
* Выключатель №1 `sm-sounds-things-switch-1`
* Выключатель №2 `sm-sounds-things-switch-2`
* Выстрел (дробовик) `sm-sounds-things-gun-1`
* Гудок корабля №1 `sm-sounds-transport-ship-horn-1`
* Гудок корабля №2 `sm-sounds-transport-ship-horn-2`
* Дверь №1 `sm-sounds-things-door-1`
* Дверь №2 `sm-sounds-things-door-2`
* Звон бокалов `sm-sounds-things-glass-2`
* Клавиатура `sm-sounds-keyboard-typing-1`
* Колокол №1 `sm-sounds-things-bell-1`
* Колокол №2 `sm-sounds-things-bell-2`
* Машина (заводится) `sm-sounds-things-car-1`
* Машина (не заводится) `sm-sounds-things-car-2`
* Меч (выходит из ножен) `sm-sounds-things-sword-2`
* Меч (парирование) `sm-sounds-things-sword-1`
* Меч (поединок) `sm-sounds-things-sword-3`
* Сирена №1 `sm-sounds-things-siren-1`
* Сирена №2 `sm-sounds-things-siren-2`
* Старый телефон №1 `sm-sounds-things-old-phone-1`
* Старый телефон №2 `sm-sounds-things-old-phone-2`
* Стекло (разбивается) `sm-sounds-things-glass-1`
* Строительство (отбойный молоток) `sm-sounds-things-construction-2`
* Строительство (пила и молоток) `sm-sounds-things-construction-1`
* Телефон №1 (звонок) `sm-sounds-things-phone-1`
* Телефон №2 (звонок) `sm-sounds-things-phone-2`
* Телефон №3 (набор номера) `sm-sounds-things-phone-3`
* Телефон №4 (гудок) `sm-sounds-things-phone-4`
* Телефон №5 (гудок) `sm-sounds-things-phone-5`
* Унитаз `sm-sounds-things-toilet-1`
* Часы с кукушкой №1 `sm-sounds-things-cuckoo-clock-1`
* Часы с кукушкой №2 `sm-sounds-things-cuckoo-clock-2`

У тега есть несколько атрибутов:

* `fade_in` - усиление.

```xml
<extra.background-audio fade_in="1.0" src="sm-sounds-human-cheer-1">
    С днём рождения! Всего самого лучшего!
</extra.background-audio>
```

Плавное увеличение громкости звука. Укажите, через сколько секунд после начала воспроизведения фоновый звук должен набрать максимальную громкость.

* `fade_out`  - затухание.

```xml
<extra.background-audio fade_out="1.0" src="sm-sounds-human-crowd-6">
    Всем спасибо, всего доброго, до новых встреч!
</extra.background-audio>
```

Плавное уменьшение громкости звука. Укажите, за сколько до конца аудио фоновый звук должен начать затихать.

* `volume` - регулировка громкости.

```xml
<extra.background-audio volume="0.5" src="sm-sounds-human-walking-dead-1">
    Что это за звук? Как будто кто-то рычит.
</extra.background-audio>
```

* `loop` - зацикливание.

```xml
<extra.background-audio loop="join" src="sm-sounds-human-cough-1">
    Больным в этой палате уже значительно лучше.
</extra.background-audio>
```

Зацикливание нужно использовать, если синтезированная речь длится дольше фонового звука — тогда фон будет воспроизводиться по кругу, снова и снова. Атрибут `loop` может принимать следующие значения:

* `none` — отсутствие зацикливания.
* `join` — зацикливание «встык». Фоновый звук снова начинает воспроизводиться сразу после окончания предыдущего фрагмента.
* `crossfade` — плавная склейка. В последнюю секунду фонового звука включается затухание и начинает звучать следующий фрагмент, у которого настроено плавное усиление в течение первой секунды.

Пример:

```xml
<speak>
    <extra.background-audio loop="crossfade" fade_out="2.0" src="sm-sounds-human-walking-dead-1">
        О, нет, это зомби! Они идут сюда. Надеюсь, они нас не найдут. 
        На самом деле, это все нужно только для того, чтобы услышать зацикленный кроссфэйд.
        Потому что звук зомби длится всего 4 секунды. В конце звук плавно затихает.
    </extra.background-audio>
    <extra.background-audio loop="crossfade" volume="0.5" fade_out="1.0" src="sm-sounds-human-walking-dead-1">
        Кстати, зомби могут реветь и потише.
    </extra.background-audio>
</speak>
```


