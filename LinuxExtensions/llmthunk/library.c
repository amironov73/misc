#include <stdio.h>
#include <stdlib.h>
#include <sys/stat.h>
#include <uuid/uuid.h>

#include "llm.h"
#include <string.h>

// Экспортированная функция
EXPORT int DLL_CALL Run
    (
        const char *input,
        char *output,
        int size
    )
{
    NOT_USED (input);

    const char *prefix_dir = "workdir/iogunb/";
    uuid_t binuuid;
    char uuid_str[37];
    char input_filename[64];
    char output_filename[64];
    char command_line[256];
    int unused;

    mkdir (prefix_dir, 0755);

    // 1. Генерируем один уникальный ID для пары файлов
    uuid_generate_random (binuuid);
    uuid_unparse_lower (binuuid, uuid_str);

    // 2. Формируем имена файлов с использованием общего UUID
    snprintf (input_filename, sizeof (input_filename), "%sinput_%s.txt", prefix_dir, uuid_str);
    snprintf (output_filename, sizeof (output_filename), "%soutput_%s.txt", prefix_dir, uuid_str);

    FILE *inputFile = fopen (input_filename, "wt");
    int length = strlen (input);
    fwrite (inputFile, 1, length, (void*) input);
    fclose (inputFile);

    memset (command_line, 0, sizeof (command_line));
    snprintf (command_line, sizeof (command_line), "%s %s %s",
        "./llmcall", input_filename, output_filename);

    unused = system (command_line);

    memset (output, 0, size);
    FILE *outputFile = fopen (output_filename, "rt");
    if (outputFile != NULL) {
        unused = fread (outputFile, 1, size - 1, (void*) output);
        fclose (outputFile);
    }

    NOT_USED (unused);

    return 0;
}

