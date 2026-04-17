#include <stdio.h>
#include <stdlib.h>

#include "llm.h"
#include "string.h"

typedef struct {
    const char *command;
    int (*function) (const char*, char*, int);
} command_t;

static command_t commands[] = {
  {"Run", Run }
};

static void call_command
    (
        const char *command,
        const char *arguments[],
        int argc
    )
{
    NOT_USED (arguments);
    NOT_USED (argc);

    int found = 0;
    for (int i = 0; commands[i].command; i++) {
        if (same_text(command, commands[i].command)) {
            found = 1;

            int size = 32 * 1024;
            char *input = calloc (size, sizeof(char));
            char *output = calloc (size, sizeof(char));
            int returnCode = commands[i].function (input, output, size);
            printf ("%d\n", returnCode);
            printf ("%s\n", output);

            break;
        }
    }

    if (!found) {
        printf("Unknown command: %s\n", command);
    }
}

// Пример использования (для теста)
NO_EXPORT int main(int argc, const char *argv[])
{
    if (argc < 2) {
        printf("USAGE: %s <command>\n", argv[0]);
        return 1;
    }

    const char *command = argv[1];
    call_command (command, argv + 2, argc - 2);

    return 0;
}
