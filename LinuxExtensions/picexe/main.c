#include <stdio.h>

// Макрос STB_IMAGE_IMPLEMENTATION должен быть определен только в ОДНОМ .c файле
#define STB_IMAGE_IMPLEMENTATION
#include "../stb/stb_image.h"

// Пример использования (для теста)
int main(int argc, const char *argv[]) {
    if (argc != 2) {
        printf("USAGE: %s <path/to/image.png>\n", argv[0]);
        return 1;
    }

    int width, height, channels;
    const char *fileName = argv[1];

    if (stbi_info(fileName, &width, &height, &channels)) {
        printf("File: %s | Width: %d, Height: %d\n",
            fileName, width, height);
    } else {
        printf("ERROR\n");
        return 1;
    }

    return 0;
}
