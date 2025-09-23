# WordPressPages

Простая утилита для вывода перечня страниц (именно страниц, а не постов!) для указанного сайта на WordPress.

Пример командной строки для запуска:

```bash
WordPressPages https://library.istu.edu/wp-json/ > pages.csv
```

Пример вывода программы (сокращенный):

```
1 2 3 4 5 6 7 8 9 
ALL PAGES COUNT: 241

20738	План мероприятий	02.04.2024	01.11.2024	http://library.istu.edu/plan-meropriyatij/
20614	Институциональный репозиторий	17.03.2024	19.03.2024	http://library.istu.edu/institutsionalnyj-repozitorij/
12	О Научно-технической библиотеке ИРНИТУ	14.10.2011	31.03.2025	http://library.istu.edu/about-2/
22346	    Юбилей НТБ ИРНИТУ	31.03.2025	31.03.2025	http://library.istu.edu/about-2/yubilej-ntb-irnitu/
20944	    Библиотека для СВО	16.05.2024	10.06.2024	http://library.istu.edu/about-2/biblioteka-dlya-svo/
16414	    Интересно о библиотеке	31.03.2022	31.03.2022	http://library.istu.edu/about-2/interesno-o-biblioteke/
16404	        6 интересных фактов о библиотеках	31.03.2022	31.03.2022	http://library.istu.edu/about-2/interesno-o-biblioteke/6-interesnyh-faktov-o-bibliotekah/
12334	    Библиотека сегодня	09.03.2021	01.04.2025	http://library.istu.edu/about-2/bibliotekasegodnya/
8356	    О нас пишут	27.12.2017	11.06.2025	http://library.istu.edu/about-2/o-nas-pishut/
6707	    Путеводитель	15.06.2016	08.10.2024	http://library.istu.edu/about-2/putevoditel/
354	    Сайты библиотек	08.11.2011	10.01.2019	http://library.istu.edu/about-2/sites/
2245	        Государственная общественно-политическая библиотека	20.02.2012	20.02.2012	http://library.istu.edu/about-2/sites/gopb/
2165	        Национальный библиотечный ресурс	13.02.2012	13.02.2012	http://library.istu.edu/about-2/sites/natlib/
1677	        The European Library	27.12.2011	27.12.2011	http://library.istu.edu/about-2/sites/european-library/
301	    Обратная связь	08.11.2011	26.03.2019	http://library.istu.edu/about-2/feedback/
295	    Контактная информация	08.11.2011	14.02.2025	http://library.istu.edu/about-2/contacts/
293	    Структура НТБ ИРНИТУ	08.11.2011	11.07.2025	http://library.istu.edu/about-2/structure/
13101	        Отдел информационного сопровождения научной и образовательной деятельности	23.10.2023	26.10.2023	http://library.istu.edu/about-2/structure/otdel-informatsionnogo-soprovozhdeniya-nauchnoj-i-obrazovatelnoj-deyatelnosti/
13079	        Отдел хранения библиотечных фондов	14.05.2021	06.06.2024	http://library.istu.edu/about-2/structure/sektor-hraneniya-knizhnogo-fonda/
13073	        Филиалы НТБ ИРНИТУ	14.05.2021	20.07.2021	http://library.istu.edu/about-2/structure/filialy-ntb-irnitu/
11203	        Президентская библиотека им. Б.Н. Ельцина	30.09.2020	07.02.2025	http://library.istu.edu/about-2/structure/prezidentskoj-biblioteki-im-b-n-eltsina/
6775	        Отдел формирования фондов и организации каталогов	15.06.2016	06.06.2024	http://library.istu.edu/about-2/structure/otdel-komplektovaniya-literaturoj/
6769	        Абонемент художественной литературы	15.06.2016	20.10.2020	http://library.istu.edu/about-2/structure/abonement-xudozhestvennoj-literatury/
6868	            Электронные библиотеки России и СНГ	21.06.2016	21.06.2016	http://library.istu.edu/about-2/structure/abonement-xudozhestvennoj-literatury/elektronnye-biblioteki-rossii-i-sng/
136	            Литературные премии	27.10.2011	27.10.2011	http://library.istu.edu/about-2/structure/abonement-xudozhestvennoj-literatury/prizes/
132	            Электронные библиотеки художественной литературы	27.10.2011	27.10.2011	http://library.istu.edu/about-2/structure/abonement-xudozhestvennoj-literatury/electronic-libraries/
6749	        Центр образовательных ресурсов (ЦОР)	15.06.2016	20.12.2018	http://library.istu.edu/about-2/structure/centr-obrazovatelnyx-resursov-cor/
11052	            Удаленный электронный читальный зал  Президентской библиотеки имени Б.Н. Ельцина	15.09.2020	24.01.2022	http://library.istu.edu/about-2/structure/centr-obrazovatelnyx-resursov-cor/virtualnyj-chitalnyj-zal-prezidentskoj-biblioteki-imeni-b-n-eltsina/
15752	                ФИЛЬМЫ, ВИДЕОЛЕКЦИИ И ВЕБИНАРЫ	21.01.2022	21.01.2022	http://library.istu.edu/about-2/structure/centr-obrazovatelnyx-resursov-cor/virtualnyj-chitalnyj-zal-prezidentskoj-biblioteki-imeni-b-n-eltsina/filmy-videolektsii-i-vebinary/
6744	        Центр научной информации (ЦНИ)	15.06.2016	20.10.2020	http://library.istu.edu/about-2/structure/centr-nauchnoj-informacii-cni/
6740	        Отдел библиотечного обслуживания	15.06.2016	06.06.2024	http://library.istu.edu/about-2/structure/otdel-bibliotechnogo-obsluzivania/
6657	        Отдел редких книг и литературы по искусству	14.06.2016	19.11.2021	http://library.istu.edu/about-2/structure/otdel-redkix-knig-i-literatury-po-iskusstvu/
5680	            Обзорная лекция о книгах Отдела редкой книги и литературы по искусству	05.10.2015	05.02.2022	http://library.istu.edu/about-2/structure/otdel-redkix-knig-i-literatury-po-iskusstvu/obzornaya-lekciya-o-knigax-otdela-redkoj-knigi-i-literatury-po-iskusstvu/
5078	            Обзоры мероприятий	18.03.2015	18.03.2015	http://library.istu.edu/about-2/structure/otdel-redkix-knig-i-literatury-po-iskusstvu/obzory-vystavok/
1759	        Отдел автоматизации	27.12.2011	20.12.2018	http://library.istu.edu/about-2/structure/automation/
274	    Наши проекты	08.11.2011	09.07.2025	http://library.istu.edu/about-2/projects/
280	        Экология Байкала и Байкальского региона	08.11.2011	05.02.2022	http://library.istu.edu/about-2/projects/ecology/
278	        История инженерного дела	08.11.2011	05.02.2022	http://library.istu.edu/about-2/projects/history-of-engineering/
276	        Проект <Мнемозина>	08.11.2011	05.02.2022	http://library.istu.edu/about-2/projects/mnemosine/
251	    Партнеры НТБ ИРНИТУ	07.11.2011	22.03.2021	http://library.istu.edu/about-2/partners/
240	    Режим работы НТБ ИРНИТУ	07.11.2011	03.09.2024	http://library.istu.edu/about-2/regime/
148	    История библиотеки	28.10.2011	22.03.2021	http://library.istu.edu/about-2/history/
297	        Владимир Сергеевич Манассеин - первый директор библиотеки	08.11.2011	10.04.2018	http://library.istu.edu/about-2/history/manassein/
144	    Дары и дарители	27.10.2011	10.01.2019	http://library.istu.edu/about-2/gifts/
2993	        Геннадий Иннокентьевич Хвощевский	04.06.2012	04.06.2012	http://library.istu.edu/about-2/gifts/hvoshhevskiy/
1580	        Польские исследователи Сибири	26.12.2011	26.12.2011	http://library.istu.edu/about-2/gifts/poland-siberi/
1574	        Казанский кафедральный собор	25.12.2011	25.12.2011	http://library.istu.edu/about-2/gifts/kazan-cathedral/
1562	        Красный сарафан	25.12.2011	25.12.2011	http://library.istu.edu/about-2/gifts/red-sundress/
1434	        Петр Васильевич Виниченко	20.12.2011	20.12.2011	http://library.istu.edu/about-2/gifts/vinichenko/
874	        Кольцо Прометея	09.12.2011	09.12.2011	http://library.istu.edu/about-2/gifts/prometeus-ring/
```
