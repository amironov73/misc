#include <stdio.h>
#include <string.h>
#include <stdlib.h>

#include "ipdf.h"

static const char* memfind
    (
        const char *buffer,
        int bufferSize,
        const char *pattern,
        int patternSize
    )
{
    //printf ("Buffer size: %d\n", bufferSize);
    //printf ("Pattern size: %d\n", patternSize);
    bufferSize -= patternSize;

    for (int i = 0; i < bufferSize; i++) {
        int found = 1;
        for (int j = 0; j < patternSize; j++) {
            if (buffer[i + j] != pattern[j]) {
                found = 0;
                break;
            }
        }
        if (found) {
            return buffer + i;
        }
    }

    return NULL;
}

static const char* memstr
    (
        const char *buffer,
        int bufferSize,
        const char *string
    )
{
    return memfind (buffer, bufferSize, string, strlen (string));
}

NO_EXPORT int get_pdf_page_count
    (
        const char *filename
    )
{
    char fixedname[1024], *ptr;
    memset (fixedname, 0, sizeof (fixedname));
    strncpy (fixedname, filename, sizeof (fixedname));
    for (ptr = fixedname; *ptr; ptr++) {
        if (*ptr == '\\') {
            *ptr = '/';
        }
    }

    FILE *f = fopen (fixedname, "rb");
    if (!f) {
        // printf ("Unable to open file %s\n", filename);
        return -1;
    }

    fseek (f, 0, SEEK_END);
    long size = ftell (f);
    // printf ("File size: %ld\n", size);
    fseek (f, 0, SEEK_SET);

    char *buffer = (char *) malloc (size);
    size_t bytesRead = fread (buffer, 1, size, f);
    // printf ("Bytes read: %ld\n", bytesRead);
    fclose (f);

    int result = -1;
    const char *found = memstr (buffer, bytesRead, "/Count");
    if (found) {
        result = atoi (found + 7);
    }

    free (buffer);

    return result;
}
