#ifndef LLMTHUNK_PATH_H
#define LLMTHUNK_PATH_H

#ifndef PATH_MAX
#define PATH_MAX 1024
#endif

void path_convert_slashes (char *path);

const char *path_get_extension (const char *path);
char       *path_get_filename  (const char *path);
char       *path_get_directory (const char *path);
void        path_combine       (char *output, ...);

#endif //LLMTHUNK_PATH_H