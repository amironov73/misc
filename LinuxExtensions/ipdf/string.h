#ifndef LLMTHUNK_STRING_H
#define LLMTHUNK_STRING_H

#include <stdlib.h>

int    char_one_of      (char one, const char *many);
int    same_char        (int first, int second);
int    same_text        (const char *first, const char *second);
int    strblank         (const char *str);
int    strchg           (char *str, char oldch, char newch);
char   strlast          (const char *str);
size_t strocc           (char *str, int ch);
int    str_contains     (const char *str, char chr);
int    str_one_of       (const char *one, ...);
int    str_safe_compare (const char *first, const char *second);

const char* str_to_visible (const char *text);

unsigned int fastParse32 (const char *text, size_t length);


#endif //LLMTHUNK_STRING_H