#include <stdlib.h>
#include <assert.h>
#include <ctype.h>
#include <stdarg.h>
#include <string.h>

#include "ipdf.h"
#include "string.h"

/**
 * Совпадают ли символы с точностью до регистра?
 *
 * @param first
 * @param second
 * @return
 */
NO_EXPORT int same_char
    (
        int first,
        int second
    )
{
    return toupper (first) == toupper (second);
}

/**
 * Совпадают ли строки с точностью до регистра?
 *
 * @param first
 * @param second
 * @return
 */
NO_EXPORT int same_text
    (
        const char *first,
        const char *second
    )
{
    int result;
    char c1, c2;

    assert (first != NULL);
    assert (second != NULL);

    for (;;) {
        c1 = *first++;
        c2 = *second++;
        result = toupper (c1) - toupper (c2);
        if (result != 0) {
            return result;
        }

        if (!c1) {
            return 0;
        }
    }
}

/**
 * Determines if a given string is blank (contains
 * whitespace only).
 *
 * @param str string to check.
 * @return Zero if the string is not blank, otherwise it is.
 */
NO_EXPORT int strblank
    (
        const char *str
    )
{
    const char *ptr = str;

    while (*ptr) {
        if (!isspace (*ptr)) {
            return 0;
        }
        ++ptr;
    }

    return 1;
}

/**
 * Finds all letters in a string matching one character
 * and replaces them with another.
 *
 * @param str string to search.
 * @param oldch character to search for.
 * @param newch character to replace with.
 * @return The number of matches found.
 */
NO_EXPORT int strchg
    (
        char *str,
        char oldch,
        char newch
    )
{
    int result = 0;
    char *ptr = str;

    while (*ptr) {
        if (*ptr == oldch) {
            *ptr = newch;
            ++result;
        }
        ++ptr;
    }

    return result;
}

/**
 * Last char of the string.
 *
 * @param str string to examine
 * @return The last character or '\0'.
 */
NO_EXPORT char strlast
    (
        const char *str
    )
{
    if (str == NULL) {
        return '\0';
    }

    const char *ptr = str + strlen (str);
    if (ptr >= str) {
        return *(ptr - 1);
    }

    return '\0';
}

/**
 * Returns the number of occurrences of a given character
 * in a string.
 *
 * @param str string to search.
 * @param ch character to look for.
 * @return The number of occurrences of the given character
 * in the string.
 */
NO_EXPORT size_t strocc
    (
        char *str,
        int ch
    )
{
    size_t result = 0;
    char *ptr = str;

    while (*ptr) {
        if (*ptr == ch) {
            ++result;
        }
        ++ptr;
    }

    return result;
}

/**
 * Содержит ли данная строка хотя бы один указанный символ?
 *
 * @param str Строка для проверки.
 * @param chr Искомый символ.
 * @return Результат проверки.
 */
NO_EXPORT int str_contains
    (
        const char *str,
        char chr
    )
{
    assert (str != NULL);

    while (*str) {
        if (*str == chr) {
            return 1;
        }

        ++str;
    }

    return 0;
}

/**
 * Быстрый грязный разбор 32-битного целого без знака.
 *
 * @param text
 * @param length
 * @return
 */
NO_EXPORT unsigned int fastParse32
    (
        const char *text,
        size_t length
    )
{
    unsigned result = 0;

    while (length--) {
        result = result * 10 + (*text++ - '0');
    }

    return result;
}

/**
 * Является ли символ одним из перечисленных.
 *
 * @param one Проверяемый символ.
 * @param many Перечисленные символы.
 * @return
 */
NO_EXPORT int char_one_of
    (
        char one,
        const char *many
    )
{
    assert (many != NULL);

    while (*many) {
        if (same_char (one, *many)) {
            return 1;
        }

        ++many;
    }

    return 0;
}

NO_EXPORT int str_one_of
    (
        const char *one,
        ...
    )
{
    int result = 0;
    const char *ptr;
    va_list args;

    assert (one != NULL);

    va_start (args, one);

    while ((ptr = va_arg (args, const char*)) != NULL) {
        if (same_text (one, ptr)) {
            return 1;
        }
    }

    va_end (args);

    return result;
}

NO_EXPORT int str_safe_compare
    (
        const char *first,
        const char *second
    )
{
    if (first == NULL) {
        if (second == NULL) {
            return 1;
        }

        return -1;
    }

    if (second == NULL) {
        return 1;
    }

    return strcmp (first, second);
}

/**
 * Превращает строку в видимую.
 *
 * @param text Строка.
 * @return Видимая строка, например, "null".
 */
NO_EXPORT const char* str_to_visible
    (
        const char *text
    )
{
    const char *ptr;

    if (text == NULL) {
        return "(null)";
    }

    if (*text == 0) {
        return "(empty)";
    }

    for (ptr = text; *ptr; ++ptr) {
        if (*ptr > ' ') {
            break;
        }
    }

    if (*ptr == 0) {
        return "(blank)";
    }

    return text;
}
