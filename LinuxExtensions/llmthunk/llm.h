#ifndef LLMTHUNK_H
#define LLMTHUNK_H

#ifdef _WIN32

#define DLL_CALL __stdcall
#define EXPORT /* пусто */
#define NO_EXPORT

#else

#define DLL_CALL /* пусто */
#define EXPORT __attribute__((visibility("default")))
#define NO_EXPORT __attribute__((visibility("hidden")))

#endif

#define NOT_USED(__x) ((void)__x)

int Test (const char *input, char *output, int size);

#endif //LLMTHUNK_H