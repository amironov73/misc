#include <windows.h>
#include <shlobj_core.h>

#define NOT_USED(__x) ((void)__x)

#define EXECUTABLE_NAME "llmcall.exe"
#define INPUT_FILE_NAME "llm_input.txt"
#define OUTPUT_FILE_NAME "llm_output.txt"
#define MUTEX_NAME "KrayGemma"

static char _workdir[MAX_PATH];
static char _executable[MAX_PATH];
static char _inputFileName[MAX_PATH];
static char _outputFileName[MAX_PATH];
static char _commandLineArguments[MAX_PATH];

// #define USE_MUTEX // использовать мьютекс для ограничения одновременного запуска

#ifdef USE_MUTEX

static HANDLE hMutex;

#endif

#define OUR_API __stdcall

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

    switch (ul_reason_for_call)
    {
        case DLL_PROCESS_ATTACH:

#ifdef USE_MUTEX

            hMutex = CreateMutexA
            (
                NULL, // не наследуется потомками
                FALSE, // не захватывается сразу при создании
                MUTEX_NAME
            );

            if (hMutex == INVALID_HANDLE_VALUE)
            {
                return FALSE;
            }

#endif

            break;

        case DLL_THREAD_ATTACH:

#ifdef USE_MUTEX

            CloseHandle (hMutex);

#endif

            break;

        case DLL_THREAD_DETACH:
        case DLL_PROCESS_DETACH:
            break;

        default:
            break;
    }

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
        char fileName[MAX_PATH];

        ClearMemory (fileName, MAX_PATH);
        lstrcpyA (fileName, _workdir);
        lstrcatA (fileName, "cant_run.txt");
        WriteTextData (fileName, commandLine);

        return FALSE;
    }

    DWORD wait = WaitForSingleObject
        (
            processInfo.hProcess,
            INFINITE
        );

    if (wait == WAIT_FAILED)
    {
        char fileName[MAX_PATH];

        ClearMemory (fileName, MAX_PATH);
        lstrcpyA (fileName, _workdir);
        lstrcatA (fileName, "wait_failed.txt");
        WriteTextData (fileName, commandLine);

        CloseHandle (processInfo.hProcess);
        CloseHandle (processInfo.hThread);

        return FALSE;
    }

    DWORD exitCode;
    if (!GetExitCodeProcess (processInfo.hProcess, &exitCode))
    {
        char fileName[MAX_PATH];

        ClearMemory (fileName, MAX_PATH);
        lstrcpyA (fileName, _workdir);
        lstrcatA (fileName, "cant_get_exit_code.txt");
        WriteTextData (fileName, commandLine);

        CloseHandle (processInfo.hProcess);
        CloseHandle (processInfo.hThread);

        return FALSE;
    }

    CloseHandle (processInfo.hProcess);
    CloseHandle (processInfo.hThread);

    return exitCode == 0;
}

//====================================================================

static void DetermineWorkDirectory
    (
        const char *buffer
    )
{
    NOT_USED (buffer);

    ClearMemory (_workdir, MAX_PATH);

    if (DirectoryExists ("C:\\irbiswrk"))
    {
        lstrcpyA (_workdir, "C:\\irbiswrk\\");
        return;
    }

    // C:\Users\user\OneDrive\Документы
    if (FAILED (SHGetFolderPathA
        (
            NULL, // window
            CSIDL_MYDOCUMENTS,
            NULL, // token
            0, // flags
            _workdir
        )))
    {
        GetTempPathA (MAX_PATH, _workdir);
    }

    lstrcatA (_workdir, "\\IOGUNB");

    if (!DirectoryExists (_workdir))
    {
        CreateDirectoryA
            (
                _workdir,
                NULL // no security attributes
            );
    }

    lstrcatA (_workdir, "\\");
}

static void DetermineFileNames (void)
{
    char candidate[MAX_PATH];

    // предполагается, что _workdir содержит конечный слэш

    lstrcpyA (_executable, EXECUTABLE_NAME);
    ClearMemory (candidate, MAX_PATH);
    if (GetEnvironmentVariableA ("LLMCALL", candidate, MAX_PATH))
    {
        lstrcpyA (_executable, candidate);
    }

    lstrcpyA (_inputFileName, _workdir);
    lstrcatA (_inputFileName, INPUT_FILE_NAME);

    lstrcpyA (_outputFileName, _workdir);
    lstrcatA (_outputFileName, OUTPUT_FILE_NAME);

    lstrcpyA (_commandLineArguments, _executable);
    lstrcatA (_commandLineArguments, " -i \"");
    lstrcatA (_commandLineArguments, _inputFileName);
    lstrcatA (_commandLineArguments, "\" -o \"");
    lstrcatA (_commandLineArguments, _outputFileName);
    lstrcatA (_commandLineArguments, "\"");
}

//====================================================================

// Для тестирования передачи аргументов
__declspec (dllexport)
int __cdecl TestCdecl
    (
        const char *inputBuffer,
        char *outputBuffer,
        int outputBufferSize
    )
{
    ClearMemory (outputBuffer, outputBufferSize);

    lstrcpyA (outputBuffer, inputBuffer);
    lstrcatA (outputBuffer, " -> ");
    lstrcatA (outputBuffer, "CDECL");

    return 0;
}

// Для тестирования передачи аргументов
#pragma comment(linker, "/export:TestStdcall=_TestStdcall@12")
int __stdcall TestStdcall
    (
        const char *inputBuffer,
        char *outputBuffer,
        int outputBufferSize
    )
{
    ClearMemory (outputBuffer, outputBufferSize);

    lstrcpyA (outputBuffer, inputBuffer);
    lstrcatA (outputBuffer, " -> ");
    lstrcatA (outputBuffer, "STDCALL");

    return 0;
}

// Для тестирования передачи аргументов
#pragma comment(linker, "/export:TestWinapi=_TestWinapi@12")
int WINAPI TestWinapi
    (
        const char *inputBuffer,
        char *outputBuffer,
        int outputBufferSize
    )
{
    ClearMemory (outputBuffer, outputBufferSize);

    lstrcpyA (outputBuffer, inputBuffer);
    lstrcatA (outputBuffer, " -> ");
    lstrcatA (outputBuffer, "WINAPI");

    return 0;
}

//====================================================================

/*

   &uf('+8llmthunk,Bleep')

 */

// Чисто пошуметь в отладочных целях
#pragma comment(linker, "/export:Bleep=_Bleep@12")
int OUR_API Bleep
    (
        const char *inputBuffer,
        char *outputBuffer,
        int outputBufferSize
    )
{
    ClearMemory (outputBuffer, outputBufferSize);

    Beep (750, 300);
    MessageBeep (0xFFFFFFFF); // A simple beep.
    // If the sound card is not available, the sound is generated using the speaker.

    return 0;
}

//====================================================================

/*

   &uf('+8llmthunk,Show,Всё пропало!')

 */

// Вывод сообщения в стандартном окне
#pragma comment(linker, "/export:Show=_Show@12")
int OUR_API Show
    (
        const char *inputBuffer,
        char *outputBuffer,
        int outputBufferSize
    )
{
    ClearMemory (outputBuffer, outputBufferSize);

    int utf8Length = lstrlenA (inputBuffer);
    int wideLength = MultiByteToWideChar
        (
            CP_UTF8,
            0,
            inputBuffer,
            utf8Length,
            NULL,
            0
        );
    int wideBufferSize = (wideLength + 1) * (int) sizeof (wchar_t);
    wchar_t *wideBuffer = (wchar_t*) GlobalAlloc (GMEM_FIXED, wideBufferSize);
    ClearMemory ((char*) wideBuffer, wideBufferSize);
    MultiByteToWideChar
        (
            CP_UTF8,
            0,
            inputBuffer,
            utf8Length,
            wideBuffer,
            wideLength
        );

    MessageBoxW
        (
            NULL, // hwnd
            wideBuffer,
            L"IRBIS64",
            MB_ICONINFORMATION
        );

    GlobalFree (wideBuffer);

    return 0;
}


//====================================================================

static int RunCore
    (
        const char *inputBuffer,
        char *outputBuffer,
        int outputBufferSize
    )
{
    ClearMemory (outputBuffer, outputBufferSize);

#ifdef USE_MUTEX

    DWORD waitResult = WaitForSingleObject
        (
            hMutex,
            0 // Задаёт таймаут в миллисекундах.
            // Значение 0 означает, что функция немедленно вернёт результат,
            // не ожидая освобождения мьютекса.
        );

    // Возможные результаты:
    // * WAIT_OBJECT_0 — мьютекс успешно захвачен, можно выполнять критичные операции.
    // * WAIT_TIMEOUT — мьютекс занят другим экземпляром, приложение отказывает в выполнении операций.
    // * Другие значения указывают на ошибку, которую можно проверить через GetLastError().

    if (waitResult != WAIT_OBJECT_0)
    {
        return 1;
    }

#endif

    DetermineWorkDirectory (inputBuffer);
    DetermineFileNames();

    if (FileExists (_outputFileName))
    {
        DeleteFileA (_outputFileName);
    }

    if (!WriteTextData (_inputFileName, inputBuffer))
    {
        return 1;
    }

    if (!RunProcess (_commandLineArguments))
    {
        return 1;
    }

    if (!ReadTextData (_outputFileName, outputBuffer, outputBufferSize))
    {
        return 1;
    }

#ifdef USE_MUTEX

    ReleaseMutex (hMutex);

#endif

    return 0;
}

/*
   &uf('+8llmthunk,Run,Далеко ли до Луны?',#,'*****')

   или

   &uf('+8llmthunk,Run,',&uf('6promptaiforimages'))

 */

// Экспортируемая функция, вызываемая АРМом.
// inputBuffer – передаваемые данные (входные),
// outputBuffer – возвращаемые данные (выходные),
// outputBufferSize – размер выходного буфера (outputBuffer).
// Как правило, размер выходного буфера составляет 32000 байт.
// В ИРБИС64 данные передаются и возвращаются в UTF8.
// Переводы строки стандартные для Windows: \r\n
// Возвращаемое значение: 0 – нормальное завершение;
// любое другое значение – ненормальное.
#pragma comment(linker, "/export:Run=_Run@12")
int OUR_API Run
    (
        const char *inputBuffer,
        char *outputBuffer,
        int outputBufferSize
    )
{
    return RunCore (inputBuffer, outputBuffer, outputBufferSize);
}

// то, же, что и Run, только CDECL
__declspec(dllexport)
int __cdecl Exec
        (
                const char *inputBuffer,
                char *outputBuffer,
                int outputBufferSize
        )
{
    return RunCore (inputBuffer, outputBuffer, outputBufferSize);
}

#pragma comment(linker, "/export:Debug=_RunDebug@12")
int OUR_API RunDebug
    (
        const char *inputBuffer,
        char *outputBuffer,
        int outputBufferSize
    )
{
    ClearMemory (outputBuffer, outputBufferSize);

    DetermineWorkDirectory (inputBuffer);
    DetermineFileNames();

    if (FileExists (_outputFileName))
    {
        DeleteFileA (_outputFileName);
    }

    if (!WriteTextData (_inputFileName, inputBuffer))
    {
        lstrcpyA (outputBuffer, "File write failure");
        return 1;
    }

    lstrcpyA (outputBuffer, "Debug success");

    return 0;
}

static const char* lstrchrA
    (
        const char *text,
        char chr
    )
{
    while (*text)
    {
        if (*text == chr)
        {
            return text;
        }

        text++;
    }

    return NULL;
}

#pragma comment(linker, "/export:Write=_WriteText@12")
int OUR_API WriteText
    (
        const char *inputBuffer,
        char *outputBuffer,
        int outputBufferSize
    )
{
    ClearMemory (outputBuffer, outputBufferSize);

    const char *comma = lstrchrA (inputBuffer, ',');
    if (!comma)
    {
        return 1;
    }

    char fileName[MAX_PATH];

    ClearMemory (fileName, MAX_PATH);
    lstrcpynA (fileName, inputBuffer, comma - inputBuffer + 1);

    WriteTextData (fileName, comma + 1);

    return 0;
}

#pragma comment(linker, "/export:Cwd=_Cwd@12")
int OUR_API Cwd
        (
                const char *inputBuffer,
                char *outputBuffer,
                int outputBufferSize
        )
{
    ClearMemory (outputBuffer, outputBufferSize);

    GetCurrentDirectoryA (outputBufferSize, outputBuffer);

    return 0;
}

