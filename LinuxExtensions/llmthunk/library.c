#include <unistd.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/stat.h>
#include <sys/wait.h>
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

// На будущее: функция, которая запускает внешнюю программу
NO_EXPORT int run_external_program
    (
        const char *program,
        const char *in,
        const char *out,
        char *output,
        int size
    )
{
    char buffer[256];
    pid_t pid = fork();

    if (pid == -1) {
        SayError (output, size, "fork failed");
        return -1;
    }

    if (pid == 0) {
        // Это дочерний процесс
        // Вызываем программу. Аргументы: путь, arg0, arg1, arg2, NULL
        execlp (program, program, in, out, (char *) NULL);

        // Если execlp вернул управление — значит произошла ошибка
        SayError (output, size, "execlp failed");
        perror ("execlp failed");
        exit (EXIT_FAILURE);

    } else {

        // Это родительский процесс (наше расширение)
        int status;
        // Ждем завершения именно нашей программы
        if (waitpid (pid, &status, 0) == -1) {
            SayError (output, size, "Error during waitpid");
            return -1;
        }

        if (WIFEXITED (status)) {
            int exit_code = WEXITSTATUS (status);
            if (exit_code != 0) {
                snprintf
                    (
                        buffer,
                        sizeof (buffer),
                        "Child process exited with code %d",
                        exit_code
                    );
                SayError (output, size, buffer);
                return exit_code;
            }

            return 0; // Все отлично
        }

        if (WIFSIGNALED (status)) {
            snprintf
                (
                    buffer,
                    sizeof (buffer),
                    "Child process killed by signal %d",
                    WTERMSIG (status)
                );
            SayError (output, size, buffer);
            return -1;
        }
    }

    SayError (output, size, "General failure");

    return -1;
}

// Тест, работает ли расширение вообще
EXPORT int DLL_CALL Test
    (
        const char *input,
        char *output,
        int size
    )
{
    NOT_USED (input);

    memset (output, 0, size);
    snprintf (output, size, "%s", "The test was passed successfully.");

    return 0;
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

    // Проверяем существование файла с программой через stat
    if (stat (llmcall, &stat_buffer)) {
        snprintf (command_line, sizeof (command_line), "Can't stat file %s", llmcall);
        return SayError (output, size, command_line);
    }

    // Вдруг это не файл (ну, мало ли)?
    if (!S_ISREG (stat_buffer.st_mode)) {
        snprintf (command_line, sizeof (command_line), "Is not regular file: %s", llmcall);
        return SayError (output, size, command_line);
    }

    // Можно ли это запускать?
    if (!can_execute (&stat_buffer)) {
        snprintf (command_line, sizeof (command_line), "Not an executable: %s", llmcall);
        return SayError (output, size, command_line);
    }

    // Создаем директорию, результат проверяется ниже
    mkdir (prefix_dir, 0755);
    if (stat (prefix_dir, &stat_buffer)) {
        snprintf (command_line, sizeof (command_line), "Can't stat dir %s", prefix_dir);
        return SayError (output, size, command_line);
    }

    // Вдруг это не директория (ну, мало ли)?
    if (!S_ISDIR (stat_buffer.st_mode)) {
        snprintf (command_line, sizeof (command_line), "Is not dir: %s", prefix_dir);
        return SayError (output, size, command_line);
    }

    // Мы сможем туда писать?
    if (access (prefix_dir, W_OK|X_OK)) {
        snprintf (command_line, sizeof (command_line), "Can't write to: %s", prefix_dir);
        return SayError (output, size, command_line);
    }

    // Генерируем один уникальный ID для пары файлов
    uuid_generate_random (binuuid);
    uuid_unparse_lower (binuuid, uuid_str);

    // Формируем имена файлов с использованием общего UUID
    snprintf (input_filename, sizeof (input_filename), "%s/input_%s.txt", prefix_dir, uuid_str);
    snprintf (output_filename, sizeof (output_filename), "%s/output_%s.txt", prefix_dir, uuid_str);

    // Записываем входящий файл
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

    // Перестраховываемся: удаляем выходной файл, если он вдруг существует
    rc = unlink (output_filename);
    NOT_USED (rc); // результат не проверяем

    // Формируем командную строку
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

    // Запускаем программу и дожидаемся ее завершения
    rc = system (command_line);
    if (rc != 0) {
        snprintf (command_line, sizeof (command_line), "Can't execute %s", llmcall);
        return SayError (output, size, command_line);
    }

    if (stat (output_filename, &stat_buffer)) {
        snprintf (command_line, sizeof (command_line), "Can't stat %s", output_filename);
        return SayError (output, size, command_line);
    }

    if (!S_ISREG (stat_buffer.st_mode)) {
        snprintf (command_line, sizeof (command_line), "Is not regular file: %s", output_filename);
        return SayError (output, size, command_line);
    }

    length = (int) stat_buffer.st_size;
    if (length <= 0) {
        snprintf (command_line, sizeof (command_line), "Bad file %s", output_filename);
        return SayError (output, size, command_line);
    }

    if (length >= size) {
        snprintf (command_line, sizeof (command_line), "File %s is too long: %d", output_filename, length);
        return SayError (output, size, command_line);
    }

    memset (output, 0, size);
    FILE *outputFile = fopen (output_filename, "r");
    if (!outputFile) {
        snprintf (command_line, sizeof (command_line), "Can't open output file %s", output_filename);
        return SayError (output, size, command_line);
    }

    if (outputFile != NULL) {
        rc = fread (output, 1, length, outputFile);
        fclose (outputFile);

        if (rc != length) {
            snprintf (command_line, sizeof (command_line), "Error reading output file %s", output_filename);
            return SayError (output, size, command_line);
        }
    }

    return 0;
}

