#ifndef IPDF_H
#define IPDF_H

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

int Count (const char *input, char *output, int size);
int get_pdf_page_count (const char *filename);

#endif //IPDF_H