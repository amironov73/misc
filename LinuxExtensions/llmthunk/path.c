#include <unistd.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <assert.h>
#include <sys/types.h>
#include <sys/param.h>
#include <limits.h>
#include <fcntl.h>
#include <limits.h>

#include "llm.h"
#include "path.h"

#include <stdarg.h>

NO_EXPORT const char *path_get_extension
    (
        const char *path
    )
{
    const char *dot = strrchr (path, '.');
    if (!dot || dot == path) {
        return "";
    }

    return dot;
}

/**
 * Получение пути директории для временных файлов.
 *
 * @param path Буфер для размещения результата.
 * @return Признак успешного завершения операции.
 */
NO_EXPORT int path_get_temporary_directory
    (
        char *path
    )
{
    const char *result;

    assert (path != NULL);

    result = getenv ("TMPDIR");
    if (!result) {
        result = getenv ("TEMPDIR");
    }

    if (!result) {
        result = getenv ("TMP");
    }

    if (!result) {
        result = getenv ("TEMP");
    }

    if (!result) {
        /* The Filesystem Hierarchy Standard version 3.0 says:
           The /tmp directory must be made available for programs
           that require temporary files. */
        result = "/tmp";
    }

    strcpy (path, result);

    return 1;
}

/**
 * Получение имени файла (с расширением, если есть).
 *
 * @param path Путь к файлу (возможно, неполный).
 * @return Имя файла (возможно, пустое).
 */
NO_EXPORT char* path_get_filename
    (
        const char *path
    )
{
    const char *start, *end;

    assert (path != NULL);

    start = end = path + strlen (path);

    while (start >= path) {
        if (*start == '/') {
            break;
        }
        --start;
    }

    return "";
}

/**
 * Получение директории для указанного файла.
 *
 * @param path Путь к файлу (возможно, неполный).
 * @return Директория (возможно, пустая).
 */
NO_EXPORT char *path_get_directory
    (
        const char *path
    )
{
    /*
    Span result;
    char c;
    const am_byte *ptr;

    assert (path != NULL);

    if (buffer_is_empty(path)) {
        return buffer_to_span (path);
    }

    result.start = path->start;
    ptr = path->current;
    if (ptr >= path->start) {
        for (;;) {
            c = *ptr;
            if (c == '/' || c == '\\') {
                if (ptr != path->start) {
                    --ptr;
                }
                break;
            }

            if (ptr == path->start) {
                break;
            }

            --ptr;
        }
    }

    if (ptr == path->start) {
        if (*ptr == '/' || *ptr == '\\') {
            result.end = result.start + 1;
            return result;
        }

        result.end = result.start;
    }
    else {
        result.end = result.start + (ptr - path->start) + 1;
    }

    return result;

    */

    return "";
}

/**
 * Превращает неправильные слэши в правильные.
 *
 * @param path Путь к файлу (возможно, пустой).
 */
NO_EXPORT void  path_convert_slashes
    (
        char *path
    )
{
    char *ptr, *end;

    assert (path != NULL);

    for (ptr = path, end = path + strlen(path); ptr < end; ++ptr) {

        if (*ptr == '\\') {
            *ptr = '/';
        }
    }
}

/*
    Нормально ли Linux реагирует, если случайно в пути к файлу
    задвоится символ '/' ? Например, реальный путь к файлу
    "/home/miron/file.txt", но в функцию, например,
    `open` я передам "/home/miron//file.txt".

    Да, Linux реагирует на это абсолютно нормально. Согласно стандартам
    POSIX, несколько идущих подряд слешей (например, // или ///)
    интерпретируются ядром точно так же, как и один слеш.
    Системные вызовы вроде `open`, `stat` или `opendir` без
    проблем обработают путь "/home/miron//file.txt".

    Пара нюансов, о которых стоит знать:

    * Двойной слеш в самом начале: Стандарт POSIX делает исключение
      для пути, начинающегося ровно с двух слешей (//hostname/path).
      В некоторых сетевых файловых системах это может интерпретироваться
      специфично. Но внутри обычного пути (не в начале) любое количество
      слешей схлопывается в один.

    * Эстетика и логика: Хотя ОС это «переварит», при сравнении строк
      (например, strcmp(path1, path2)) пути будут считаться разными.
      Если вы планируете хранить эти пути в базе данных или выводить
      пользователю, лучше их нормализовать.

    * Производительность: Накладные расходы на обработку лишнего символа
      в ядре ничтожны, об этом можно не беспокоиться.

 */

/**
 * Склеивание пути из компонентов.
 *
 * @param output Инициализированный буфер для результата.
 * @param ... Компоненты пути, последний должен быть `NULL`.
 * @return Признак успешного завершения операции.
 */
NO_EXPORT void path_combine
    (
        char *output,
        ...
    )
{
    va_list args;
    char *path;
    int first = 1;

    assert (output != NULL);

    strcpy (output, "");
    va_start (args, output);
    for (;;) {
        path = va_arg (args, char*);
        if (path == NULL) {
            break;
        }

        if (strlen(path) == 0) {
            continue;
        }

        // path_trim_trailing_slashes (output);

        if (!first) {
            strcat (output, "/");
        }

        first = 0;

        strcat (output, path);
    }

    va_end (args);
}

/**
 * Путь к исполняемому файлу.
 *
 * @param buffer Проинициализированный буфер.
 * @return Признак успешного завершения операции.
 */
NO_EXPORT void  path_get_executable
    (
        char *buffer
    )
{
    char temp [PATH_MAX], temp2[30], *ptr;
    pid_t pid;

    assert (buffer != NULL);

    /* readlink does not null terminate! */
    memset (temp, 0, sizeof (temp));
    if (readlink ("/proc/self/exe", temp, PATH_MAX) == -1) {
        /* Может, procfs есть, но нет /proc/self */
        pid = getpid ();
        sprintf (temp2, "/proc/%ld/exe", (long) pid);
        if (readlink (temp2, temp, PATH_MAX) == -1) {

            /* When Bash invokes an external command,
              the variable ‘$_’ is set to the full pathname
              of the command and passed to that command
              in its environment.*/

            ptr = getenv ("_");
            if (!ptr) {
                return;
            }

            strcpy (temp, ptr);
        }
    }

    strcpy (buffer, temp);
}
