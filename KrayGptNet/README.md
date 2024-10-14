# KrayGptNet

Простая консольная утилита для получения описаний изображений с помощью ChatGPT.

Настройки хранятся в файле `appsettings.json`:

```json
{
  "model": "gpt-4o-2024-05-13",
  "apiKey": "sk-aaaabbbbccccdddd",
  "endpoint": "https://api.proxyapi.ru/openai/v1",
  "prompt": "Опиши изображение сначала по-английски, затем по-русски",
  "balance": "https://api.proxyapi.ru/proxyapi/balance",
  "temperature": 1.0,
  "maxTokens": 512
}
```

Параметры запуска:

```
KrayGptNet <input> <prefix> <output>
```

здесь

* `input` - папка, в которой будет производиться поиск изображений. 
  В качестве изображений распознаются файлы со следующими расширениями
  (независимо от регистра символов):
 
  * '.jpg'
  * '.jpeg'
  * '.bmp'
  * '.gif'
  * '.png'
  * '.tif'
  * '.tiff'
  * '.webp'

* `prefix` - префикс, добавляемый к именам файлов, например, `"pic\"`.

* `output` - имя выходного текстового файла, например, `"tags.txt"`.

Пример:

```
KrayGptNet G:\Images\fb\fb0008 pic\fb\fb0008 output.txt
```

Пример выходного файла:

```
pic\fb\fb0008\fb0008-001.jpg
finish=Stop, prompt=108, completion=228, elapsed=00:00:04.5719667
The image shows an old black and white photograph of a car driving through a driving test course. The car, which appears to be an older model, is marked with the number "14". There are a few traffic cones indicating the path or course for the vehicle. The driver and a person inside the car can be seen. A man stands nearby, observing the car. Behind them, there are some people sitting under an umbrella. The scene appears to be taking place in a public square or an open area with buildings in the background.
На изображении старая черно-белая фотография автомобиля, проходящего курс экзамена по вождению. Автомобиль, который кажется старой моделью, отмечен номером "14". Несколько дорожных конусов указывают путь или курс для автомобиля. Водитель и человек внутри автомобиля видны. Рядом стоит мужчина, наблюдающий за автомобилем. Позади них несколько человек сидят под зонтом. Сцена, похоже, происходит на площади или открытой территории с зданиями на заднем плане.

pic\fb\fb0008\fb0008-002.jpg
finish=Stop, prompt=108, completion=240, elapsed=00:00:04.1703929
The image appears to be an old photograph, possibly from the Soviet era, depicting a driving school practice area. In the foreground, a small car is navigating through a setup of vertical poles, likely as part of a driving test or training exercise. There are two men on either side; one man is walking near the car on the left while the other is standing with his hands in his pockets on the right. In the background, there is a building with multiple windows and some trees and bushes, along with a stairway leading up to the building.
На изображении, вероятно, старая фотография, возможно, из советской эпохи, изображающая площадку для занятий в автошколе. На переднем плане небольшой автомобиль маневрирует между вертикальными столбиками, скорее всего, в рамках экзамена или учебного упражнения по вождению. Слева около машины идет мужчина, а справа стоит другой мужчина с руками в карманах. На заднем плане находится здание с множеством окон, несколько деревьев и кустарников, а также лестница, ведущая к зданию.

pic\fb\fb0008\fb0008-003.jpg
finish=Stop, prompt=108, completion=239, elapsed=00:00:04.8184885
The image depicts a public square in front of a large, multi-story building. The building has several floors with windows and a sign that is partially legible, indicating "НКАТЁ" and possibly more in Russian. There is a white car parked in the middle of the square, with a few people walking nearby. Some trees and a few people sitting on benches can be seen in the background. The scene has an old, vintage look to it, suggesting that it might have been taken several decades ago.
На изображении изображена общественная площадь перед большим, многоэтажным зданием. Здание имеет несколько этажей с окнами и вывеску, частично читаемую, на которой указано "НКАТЁ" и, возможно, что-то еще на русском языке. В центре площади припаркован белый автомобиль, рядом с ним идет несколько человек. На заднем плане видны деревья и несколько людей, сидящих на скамейках. Сцена имеет старый, винтажный вид, что может указывать на то, что фотография была сделана несколько десятилетий назад.

pic\fb\fb0008\fb0008-004.jpg
finish=Stop, prompt=108, completion=238, elapsed=00:00:04.2859152
The photograph depicts an old style, four-door sedan car, painted white, with the number "14" displayed prominently on its doors. The car appears to be participating in some sort of driving event or rally, as indicated by the presence of additional text and logos on the vehicle's body, including "Lada 1600" on the rear side window. The setting is a paved area with a building in the background, which seems to be a public space or square. There are a few people visible in the background, walking or standing.
На фотографии изображен старый четырехдверный седан белого цвета с номером "14", выходящим на двери. Автомобиль, похоже, участвует в каком-то мероприятии или ралли, о чем свидетельствуют дополнительные надписи и логотипы на кузове, включая "Лада 1600" на заднем боковом окне. Место действия - мощеная площадь с зданием на заднем плане, вероятно, общественное пространство. На заднем плане видны несколько человек, идущих или стоящих.

```
