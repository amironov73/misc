#include <stdio.h>
#include <stdlib.h>
#include <sys/stat.h>
#include <uuid/uuid.h>

#include "llm.h"
#include <string.h>

NO_EXPORT int SayError
    (
        char *output,
        int output_size,
        const char *message
    )
{
    memset (output, 0, output_size);
    snprintf (output, output_size, message);

    return 1;
}

// Экспортированная функция
EXPORT int DLL_CALL Run
    (
        const char *input,
        char *output,
        int size
    )
{
    NOT_USED (input);

    const char *prefix_dir = "workdir/iogunb";
    uuid_t binuuid;
    char uuid_str[37];
    char input_filename[64];
    char output_filename[64];
    char command_line[256];
    int rc;

    mkdir (prefix_dir, 0755);

    // 1. Генерируем один уникальный ID для пары файлов
    uuid_generate_random (binuuid);
    uuid_unparse_lower (binuuid, uuid_str);

    // 2. Формируем имена файлов с использованием общего UUID
    snprintf (input_filename, sizeof (input_filename), "%s/input_%s.txt", prefix_dir, uuid_str);
    snprintf (output_filename, sizeof (output_filename), "%s/output_%s.txt", prefix_dir, uuid_str);

    FILE *inputFile = fopen (input_filename, "wt");
    if (!inputFile) {
        return SayError (output, size, "Can't open input file");
    }

    int length = strlen (input);
    fwrite (input, 1, length, inputFile);
    fclose (inputFile);

    memset (command_line, 0, sizeof (command_line));
    snprintf (command_line, sizeof (command_line), "%s -i %s -o %s",
        "./llmcall", input_filename, output_filename);

    rc = system (command_line);
    if (rc != 0) {
        return SayError (output, size, "Error executing command");
    }

    memset (output, 0, size);
    FILE *outputFile = fopen (output_filename, "rt");
    if (!outputFile) {
        return SayError (output, size, "Can't open output file");
    }

    if (outputFile != NULL) {
        unused = fread (output, 1, size - 1, outputFile);
        fclose (outputFile);
    }

    return 0;
}

