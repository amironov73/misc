#include <unistd.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/stat.h>
#include <uuid/uuid.h>

#include "llm.h"

NO_EXPORT int SayError
    (
        char *output,
        int output_size,
        const char *message
    )
{
    memset (output, 0, output_size);
    snprintf (output, output_size, "%s", message);

    return 1;
}

// Поворот слэшей для Unix
EXPORT int DLL_CALL Slashes
    (
        const char *input,
        char *output,
        int size
    )
{
    memset (output, 0, size);

    char c;
    while ((c = *input++) != 0 && size--) {
        if (c == '\\') {
            c = '/';
        }
        *output++ = c;
    }

    return 0;
}

NO_EXPORT int can_execute
    (
        struct stat *st
    )
{
    uid_t uid = geteuid(); // Получаем ID текущего пользователя

    // 1. Если мы — владелец файла
    if (uid == st->st_uid) {
        return st->st_mode & S_IXUSR;
    }

    // 2. Если мы входим в группу владельцев файла
    gid_t gid = getegid();
    if (gid == st->st_gid) {
        return st->st_mode & S_IXGRP;
    }

    // 3. Для всех остальных
    return st->st_mode & S_IXOTH;
}

// Экспортированная функция
EXPORT int DLL_CALL Run
    (
        const char *input,
        char *output,
        int size
    )
{
    const char *prefix_dir = "workdir/iogunb";
    const char *llmcall = "./llmcall";
    uuid_t binuuid;
    char uuid_str[37];
    char input_filename[64];
    char output_filename[64];
    char command_line[256];
    struct stat stat_buffer;
    int rc;

    if (stat (llmcall, &stat_buffer)) {
        snprintf (command_line, sizeof (command_line), "Can't stat file %s", llmcall);
        return SayError (output, size, command_line);
    }

    if (!can_execute (&stat_buffer)) {
        snprintf (command_line, sizeof (command_line), "Not executable: %s", llmcall);
        return SayError (output, size, command_line);
    }

    mkdir (prefix_dir, 0755);

    // 1. Генерируем один уникальный ID для пары файлов
    uuid_generate_random (binuuid);
    uuid_unparse_lower (binuuid, uuid_str);

    // 2. Формируем имена файлов с использованием общего UUID
    snprintf (input_filename, sizeof (input_filename), "%s/input_%s.txt", prefix_dir, uuid_str);
    snprintf (output_filename, sizeof (output_filename), "%s/output_%s.txt", prefix_dir, uuid_str);

    FILE *inputFile = fopen (input_filename, "w");
    if (!inputFile) {
        snprintf (command_line, sizeof (command_line), "Can't create input file %s", input_filename);
        return SayError (output, size, command_line);
    }

    int length = strlen (input);
    rc = fwrite (input, 1, length, inputFile);
    fclose (inputFile);
    if (rc != length) {
        snprintf (command_line, sizeof (command_line), "Can't write input file %s", input_filename);
        return SayError (output, size, command_line);
    }

    memset (command_line, 0, sizeof (command_line));
    snprintf
        (
            command_line,
            sizeof (command_line),
            "%s -i %s -o %s",
            llmcall,
            input_filename,
            output_filename
        );

    rc = system (command_line);
    if (rc != 0) {
        snprintf (command_line, sizeof (command_line), "Can't execute %s", llmcall);
        return SayError (output, size, command_line);
    }

    memset (output, 0, size);
    FILE *outputFile = fopen (output_filename, "r");
    if (!outputFile) {
        snprintf (command_line, sizeof (command_line), "Can't open output file %s", output_filename);
        return SayError (output, size, command_line);
    }

    if (outputFile != NULL) {
        rc = fread (output, 1, size - 1, outputFile);
        fclose (outputFile);

        if (rc <= 0) {
            snprintf (command_line, sizeof (command_line), "Error reading output file %s", output_filename);
            return SayError (output, size, command_line);
        }
    }

    return 0;
}

