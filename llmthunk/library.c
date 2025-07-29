#include <windows.h>

#pragma ide diagnostic ignored "OCUnusedGlobalDeclarationInspection"

#define NOT_USED(__x) ((void)__x)

#define EXECUTABLE_NAME "llmcall.exe"
#define INPUT_FILE_NAME "llm_input.txt"
#define OUTPUT_FILE_NAME "llm_output.txt"

static char _workdir[256];
static char _inputFileName[256];
static char _outputFileName[256];
static char _commandLineArguments[256];

//====================================================================

BOOL APIENTRY DllMain
    (
        HMODULE hModule,
        DWORD  ul_reason_for_call,
        LPVOID lpReserved
    )
{
    NOT_USED (hModule);
    NOT_USED (ul_reason_for_call);
    NOT_USED (lpReserved);

    /*
        Никакой инициализации CRT здесь нет; только то,
        что действительно требуется библиотеке.
    */

    /*
        switch (ul_reason)
        {
            case DLL_PROCESS_ATTACH:
            case DLL_THREAD_ATTACH:
            case DLL_THREAD_DETACH:
            case DLL_PROCESS_DETACH:
                break;
        }

     */

    return TRUE;
}

//====================================================================

static void ClearMemory
    (
        char *ptr, int size
    )
{
    while (size--)
    {
        *ptr++ = 0;
    }
}

static BOOL ReadTextData
    (
        const char *fileName,
        char *buffer,
        int bufferLength
    )
{
    HANDLE file = CreateFileA
        (
            fileName,
            GENERIC_READ,
            0,
            NULL,
            OPEN_EXISTING,
            FILE_ATTRIBUTE_NORMAL,
            NULL
        );
    if (file == INVALID_HANDLE_VALUE)
    {
        return FALSE;
    }

    DWORD dataLength = GetFileSize (file, NULL);
    if (dataLength >= bufferLength)
    {
        return FALSE;
    }

    DWORD readLength;
    BOOL success = ReadFile
        (
                file,
                buffer,
                dataLength,
                &readLength,
                NULL
        );

    CloseHandle (file);

    if (!success || readLength != dataLength)
    {
        return FALSE;
    }

    return TRUE;
}

static BOOL FileExists
    (
        const char *fileName
    )
{
    DWORD dwAttrib = GetFileAttributesA (fileName);

    return (dwAttrib != INVALID_FILE_ATTRIBUTES &&
            !(dwAttrib & FILE_ATTRIBUTE_DIRECTORY));
}

static BOOL DirectoryExists
    (
        const char *fileName
    )
{
    DWORD dwAttrib = GetFileAttributesA (fileName);

    return (dwAttrib != INVALID_FILE_ATTRIBUTES &&
            (dwAttrib & FILE_ATTRIBUTE_DIRECTORY));
}

static BOOL WriteTextData
    (
        const char *fileName,
        const char *textData
    )
{
    if (FileExists (fileName))
    {
        if (!DeleteFileA (fileName))
        {
            return FALSE;
        }
    }

    HANDLE file = CreateFileA
        (
            fileName,
            GENERIC_WRITE,
            0,
            NULL,
            CREATE_ALWAYS,
            FILE_ATTRIBUTE_NORMAL,
            NULL
        );

    if (file == INVALID_HANDLE_VALUE)
    {
        return 0;
    }

    DWORD dataLength = lstrlenA (textData);
    DWORD written;
    BOOL success = WriteFile
        (
            file,
            textData,
            dataLength,
            &written,
            NULL
        );

    CloseHandle (file);

    if (!success || written != dataLength)
    {
        return FALSE;
    }

    return TRUE;
}

static BOOL RunProcess
    (
        const char *commandLine
    )
{
    STARTUPINFO startupinfo;

    ClearMemory ((char*) &startupinfo, sizeof (STARTUPINFO));
    startupinfo.cb = sizeof (STARTUPINFO);

    PROCESS_INFORMATION processInfo;

    ClearMemory ((char*) &processInfo, sizeof (PROCESS_INFORMATION ));

    BOOL success = CreateProcessA
        (
            NULL, // no module name (use command line)
            (char*) commandLine, // Command line
            NULL, // Process handle not inheritable
            NULL, // Thread handle not inheritable
            FALSE, // Set handle inheritance to FALSE
            0, // No creation flags
            NULL, // Use parent's environment block
            NULL, // Use parent's starting directory
            &startupinfo, // Pointer to STARTUPINFO structure
            &processInfo // Pointer to PROCESS_INFORMATION structure
        );
    if (!success)
    {
        return FALSE;
    }

    DWORD wait = WaitForSingleObject
        (
            processInfo.hProcess,
            INFINITE
        );

    if (wait == WAIT_FAILED)
    {
        CloseHandle (processInfo.hProcess);
        CloseHandle (processInfo.hThread);

        return FALSE;
    }

    DWORD exitCode;
    if (!GetExitCodeProcess (processInfo.hProcess, &exitCode))
    {
        CloseHandle (processInfo.hProcess);
        CloseHandle (processInfo.hThread);

        return FALSE;
    }

    CloseHandle (processInfo.hProcess);
    CloseHandle (processInfo.hThread);

    return TRUE;
}

//====================================================================

static const char* FindCharacter
    (
        const char *buffer,
        char character
    )
{
    while (*buffer)
    {
        if (*buffer == character)
        {
            return buffer;
        }

        buffer++;
    }

    return NULL;
}

static int CompareWithLength
    (
        const char *first,
        const char *second,
        int length
    )
{
    int result = length > 0;

    while (length > 0)
    {
        if (*first != *second)
        {
            result = FALSE;
            break;
        }

        first++;
        second++;
        length--;
    }

    return result;
}

static void DetermineWorkDirectory
    (
        const char *buffer
    )
{
    NOT_USED (buffer);

    if (DirectoryExists ("C:\\irbiswrk"))
    {
        lstrcpyA (_workdir, "C:\\irbiswrk\\");
        return;
    }

    ClearMemory (_workdir, sizeof (_workdir));

    int length = (int) GetTempPathA (sizeof (_workdir) - 1, _workdir);
    if (length >= sizeof (_workdir))
    {
        _workdir[0] = 0;
        return;
    }

//    _workdir[0] = 0;
//
//    if (!CompareWithLength (buffer, "WORKDIR=", 8))
//    {
//        return;
//    }
//
//    const char *end = FindCharacter (buffer, '\r');
//    if (!end)
//    {
//        end = FindCharacter (buffer, '\n');
//
//        if (!end)
//        {
//            return;
//        }
//    }
//
//    int length = end - buffer - 8;
//    lstrcpynA (_workdir, buffer + 8, length);
//
//    while (length > 0)
//    {
//        if (_workdir[length - 1] != ' ')
//        {
//            break;
//        }
//
//        length--;
//    }

//    if (_workdir[length] != '\\')
//    {
//        _workdir[++length] = '\\';
//        length++;
//    }
//
//    _workdir[length] = 0;

//    while (_workdir[0] == ' ')
//    {
//        lstrcpyA (_workdir, _workdir + 1);
//    }
}

static void DetermineFileNames (void)
{
    // предполагается, что _workdir содержит конечный слэш

    lstrcpyA (_inputFileName, _workdir);
    lstrcatA (_inputFileName, INPUT_FILE_NAME);

    lstrcpyA (_outputFileName, _workdir);
    lstrcatA (_outputFileName, OUTPUT_FILE_NAME);

    lstrcpyA (_commandLineArguments, EXECUTABLE_NAME);
    lstrcatA (_commandLineArguments, " -i \"");
    lstrcatA (_commandLineArguments, _inputFileName);
    lstrcatA (_commandLineArguments, "\" -o \"");
    lstrcatA (_commandLineArguments, _outputFileName);
    lstrcatA (_commandLineArguments, "\"");
}

//====================================================================

/*
   &uf('+8llmthunk,Run,',v200^a),
   #,
   &uf('+8llmthunk,Run,',v910^a)
 */

// экспортируемая функция, вызываемая АРМом.
__declspec (dllexport)
int __cdecl Run
    (
        const char *inputBuffer,
        char *outputBuffer,
        int outputBufferSize
    )
{
    ClearMemory (outputBuffer, outputBufferSize);

    DetermineWorkDirectory (inputBuffer);
    DetermineFileNames();

    if (!WriteTextData (_inputFileName, inputBuffer))
    {
        return 0;
    }

    if (!FileExists (_outputFileName))
    {
        DeleteFileA (_outputFileName);
    }

    if (!RunProcess (_commandLineArguments))
    {
        return 0;
    }

    if (!ReadTextData (_outputFileName, outputBuffer, outputBufferSize))
    {
        return 0;
    }

    return 1;
}
