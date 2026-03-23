#include <stdio.h>
#include <string.h>

#include "ipdf.h"

// Экспортированная функция
EXPORT int DLL_CALL Count
    (
        const char *input,
        char *output,
        int size
    )
{
    if (!input || !strlen (input)) {
        return 0;
    }

    // printf ("INPUT FILE: %s\n", input);
    memset (output, 0, size);
    int count = get_pdf_page_count (input);
    sprintf (output, "%d", count);

    return 0;
}

// Создаем алиас
EXPORT int CountPages() __attribute__((alias("Count")));
