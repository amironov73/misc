#include <stdio.h>

#include "llm.h"
#include <string.h>

// Экспортированная функция
EXPORT int DLL_CALL Test
    (
        const char *input,
        char *output,
        int size
    )
{
    NOT_USED (input);

    memset (output, 0, size);
    strncpy (output, "Test Succeed", size);

    return 0;
}

