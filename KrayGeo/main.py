import requests
import csv
import time

# Вставьте ваш API-ключ Яндекс.Геокодера здесь
YANDEX_API_KEY = "?"

# URL для запросов к Яндекс.Геокодеру
YANDEX_GEOCODE_URL = "https://geocode-maps.yandex.ru/1.x/"

# Чтение списка населённых пунктов из файла
with open('places.txt', 'r', encoding='utf-8') as f:
    places = [line.strip() for line in f if line.strip()]  # Удаляем пустые строки

# Открытие CSV-файла для записи результатов
with open('coordinates.csv', 'w', encoding='utf-8', newline='') as csvfile:
    writer = csv.writer(csvfile)
    # Записываем заголовки столбцов
    writer.writerow(['place_name', 'latitude', 'longitude', 'full_address'])

    # Обработка каждого населённого пункта
    for i, place in enumerate(places, 1):
        print(f"Обработка {i}/{len(places)}: {place}")
        # Формируем запрос с указанием региона
        query = f"{place}, Иркутская область, Россия"
        try:
            # Параметры запроса
            params = {
                "apikey": YANDEX_API_KEY,
                "geocode": query,
                "format": "json",
                "lang": "ru_RU",
                "results": 1  # Получаем только один (наиболее релевантный) результат
            }
            # Отправляем запрос к Яндекс.Геокодеру
            response = requests.get(YANDEX_GEOCODE_URL, params=params)
            data = response.json()

            # Проверяем, есть ли результаты
            if data['response']['GeoObjectCollection']['metaDataProperty']['GeocoderResponseMetaData']['found'] > 0:
                # Извлекаем первый результат
                geo_object = data['response']['GeoObjectCollection']['featureMember'][0]['GeoObject']
                # Получаем координаты (долгота, широта)
                lon, lat = geo_object['Point']['pos'].split()
                # Получаем полный адрес
                address = geo_object['metaDataProperty']['GeocoderMetaData']['text']
                # Записываем в CSV
                writer.writerow([place, lat, lon, address])
                print(f'\tУспех: {place}')
            else:
                writer.writerow([place, '', '', 'Не найдено'])
                print('\tНе найдено')
        except Exception as e:
            writer.writerow([place, '', '', f'Ошибка: {str(e)}'])
            print(f'\tОшибка: {str(e)}')
        # Задержка между запросами для соблюдения лимитов
        time.sleep(1.0)

print("Геокодирование завершено. Результаты сохранены в coordinates.csv")