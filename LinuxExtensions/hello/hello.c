#include <stdio.h>
#include <string.h>

#ifdef _WIN32
#define DLL_CALL __stdcall
#else
// В Linux x86_64 это будет проигнорировано, в x86 — сработает
#define DLL_CALL __attribute__((stdcall))
#endif

int DLL_CALL WriteHello
    (
        const char *input,
        char *output,
        int size
    )
{
    memset (output,0, size);
    snprintf (output, size, "%s", "Привет, ИРБИС!");

    return 0;
}
