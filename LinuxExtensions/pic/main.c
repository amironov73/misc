#include <stdio.h>

// Отключаем предупреждение о том, что большинство функций STB мы не использовали
#pragma GCC diagnostic ignored "-Wunused-function"

// Макрос STB_IMAGE_IMPLEMENTATION должен быть определен только в ОДНОМ .c файле
#define STB_IMAGE_IMPLEMENTATION

// Нам не нужно экспортировать функции STB
#define STB_IMAGE_STATIC

#include "stb/stb_image.h"

#ifdef _WIN32

#define DLL_CALL __stdcall
#define PIC_EXPORT
#define PIC_NO_EXPORT

#else

#define DLL_CALL /* пусто */
#define PIC_EXPORT __attribute__((visibility("default")))
#define PIC_NO_EXPORT __attribute__((visibility("hidden")))

#endif

// Экспортированная функция
PIC_EXPORT int DLL_CALL Size
    (
        const char *input,
        char *output,
        int size
    )
{
    STBI_NOTUSED(input);
    memset (output,0, size);

    char filename[1024], *ptr;
    memset (filename, 0, sizeof (filename));
    strncpy (filename, input, sizeof (filename));
    for (ptr = filename; *ptr; ptr++) {
        if (*ptr == '\\') {
            *ptr = '/';
        }
    }

    int width = 0, height = 0, channels = 0;
    if (stbi_info(filename, &width, &height, &channels)) {
        snprintf(output, size, "%d,%d", width, height);
    }
    else {
        snprintf(output, size, "%s", "0,0");
    }

    return 0;
}

// Пример использования (для теста)
PIC_NO_EXPORT int main(int argc, const char *argv[]) {

    if (argc != 2) {
        printf("USAGE: %s <path/to/image.png>\n", argv[0]);
        return 1;
    }

    int width, height, channels;
    const char *fileName = argv[1];

    if (stbi_info(fileName, &width, &height, &channels)) {

        printf("File: %s | Width: %d, Height: %d, Channels: %d\n",
            fileName, width, height, channels);

    } else {

        printf("ERROR\n");
        return 1;

    }

    return 0;
}
