// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

#include <windows.h>
#include <shlobj_core.h>
#include <Shlwapi.h>

#pragma ide diagnostic ignored "OCUnusedGlobalDeclarationInspection"
#pragma ide diagnostic ignored "ConstantParameter"
#pragma ide diagnostic ignored "bugprone-branch-clone"
#pragma ide diagnostic ignored "bugprone-reserved-identifier"

#define NOT_USED(__x) ((void)__x)

#define DEFAULT_EXECUTABLE_NAME "llmcall.exe"
#define DEFAULT_INPUT_FILE_NAME "llm_input.txt"
#define DEFAULT_OUTPUT_FILE_NAME "llm_output.txt"
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

    SecureZeroMemory ((char*) &startupinfo, sizeof (STARTUPINFO));
    startupinfo.cb = sizeof (STARTUPINFO);

    PROCESS_INFORMATION processInfo;

    SecureZeroMemory ((char*) &processInfo, sizeof (PROCESS_INFORMATION ));

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

    return exitCode == 0;
}

// поиск начала строчки с заданным префиксом, например "MACHINE="
static const char *FindBeginning
    (
        const char *buffer,
        const char *prefix,
        int prefixLength
    )
{
    while (*buffer)
    {
        int found = 1;
        for (int i = 0; i < prefixLength; i++)
        {
            if (buffer[i] != prefix[i])
            {
                found = 0;
                break;
            }
        }

        if (found)
        {
            return buffer;
        }

        buffer++;
    }

    return NULL;
}

// поиск конца строки
static const char *FindEnd
    (
        const char *buffer
    )
{
    while (*buffer)
    {
        char chr = *buffer;
        if (chr == '\r' || chr == '\n')
        {
            return buffer;
        }

        buffer++;
    }

    return NULL;
}

// создание вложенной директории "IOGUNB"
static void CreateIogunbSubdir()
{
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

// определение, не клиентский ли АРМ нас загрузил
static BOOL IsClient
    (
        const char *name
    )
{
    return (BOOL)
        (
            (name[0] == 'C') || (name[0] == 'c') &&
            (name[1] == 'I') || (name[1] == 'i') &&
            (name[2] == 'R') || (name[2] == 'r') &&
            (name[3] == 'B') || (name[3] == 'b') &&
            (name[4] == 'I') || (name[4] == 'i') &&
            (name[5] == 'S') || (name[5] == 's')
        )
        ||
        (
            (name[0] == 'I') || (name[0] == 'i') &&
            (name[1] == 'R') || (name[1] == 'r') &&
            (name[2] == 'B') || (name[2] == 'b') &&
            (name[3] == 'I') || (name[3] == 'i') &&
            (name[4] == 'S') || (name[4] == 's') &&
            (name[5] == 'T') || (name[5] == 't') &&
            (name[6] == 'A') || (name[6] == 'a') &&
            (name[7] == 'B') || (name[7] == 'b')
        );
}

static char* GetFileName
    (
        char *fullPath
    )
{
    char *candidate = fullPath;
    for (char *ptr = fullPath; *ptr; ptr++)
    {
        if (*ptr == '\\' && ptr[1] != 0)
        {
            candidate = ptr + 1;
        }
    }

    return candidate;
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

static void SetupWorkDirectoryAndFiles
    (
        const char *buffer
    )
{
    // Первейшая наша задача - получить полный путь к рабочей директории.
    // Главная проблема тут - DLL может выполняться как на клиенте,
    // так и на сервере.
    // Различить их можно, запросив полный путь до программы,
    // которая нас загрузила. По ней становится ясно, под кем мы работаем.
    // Вторая наша задача - найти полный путь до llmcall.exe

    char mainModule[MAX_PATH];
    char candidate[MAX_PATH];
    char guid[MAX_PATH];
    char iniFile[MAX_PATH];

    SecureZeroMemory (_workdir, MAX_PATH);
    SecureZeroMemory (_executable, MAX_PATH);
    SecureZeroMemory (mainModule, MAX_PATH);
    SecureZeroMemory (candidate, MAX_PATH);
    SecureZeroMemory (guid, MAX_PATH);
    SecureZeroMemory (iniFile, MAX_PATH);

    // получаем полный путь до программы, которая нас загрузила
    GetModuleFileName (NULL, mainModule, MAX_PATH);
    char *exeName = GetFileName (mainModule);
    int dirLength = exeName - mainModule + 1;

    // llmcall.exe должен лежать рядом с главным модулем программы,
    // в которую мы загружены
    lstrcpynA (_executable, mainModule, dirLength);
    lstrcatA (_executable, DEFAULT_EXECUTABLE_NAME);

    if (IsClient (exeName))
    {
        // это клиент, значит, мы работаем в интерактивном режиме,
        // у нас есть папка документов пользователя типа такой:
        // C:\Users\user\OneDrive\Документы

        SHGetFolderPathA
            (
                NULL, // window
                CSIDL_MYDOCUMENTS,
                NULL, // token
                0, // flags
                _workdir
            );

        // или можно попробовать GetTempPathA (MAX_PATH, _workdir);
    }
    else
    {
        // это сервер, рядом с ним должен лежать файл irbis_server.ini,
        // в секции Main которого есть параметр workdir,
        // хранящий полный путь до рабочей директории

        lstrcpyA (iniFile, mainModule);
        PathRemoveFileSpecA (iniFile);
        lstrcatA (iniFile, "irbis_server.ini");
        GetPrivateProfileStringA
            (
                "Main", // section
                "workdir", // key name
                NULL, // default value
                _workdir, // result
                MAX_PATH, // size of result
                iniFile // ini file name
            );
    }

    // на всякий случай добавляем обратный слэш
    PathAddBackslashA (_workdir);

    // и создаем папку IOGUNB, если ее еще не было
    CreateIogunbSubdir();

    // GUID нужен, чтобы не запросы от разных пользователей не затирали друг друга
    const char *beginning = FindBeginning (buffer, "GUID=", 5);
    if (beginning)
    {
        beginning += 5;
        const char *end = FindEnd (beginning);
        if (end)
        {
            lstrcpynA (guid, beginning, end - beginning + 1);
            return;
        }
    }

    // в принципе, полный путь до llmcall.exe можно указать в переменной окружения
    if (GetEnvironmentVariableA ("LLMCALL", candidate, MAX_PATH))
    {
        // переменная должна содержать полное имя EXE-файла
        lstrcpyA (_executable, candidate);
    }

//    if (!FileExists (_executable))
//    {
//        // придумать, что делать, если исполняемый файл llmcall.exe не найден
//        lstrcpyA (_executable, DEFAULT_EXECUTABLE_NAME);
//    }

    // предполагается, что _workdir содержит конечный слэш

    // имя входного файла
    lstrcpyA (_inputFileName, _workdir);
    lstrcatA (_inputFileName, DEFAULT_INPUT_FILE_NAME);
    if (guid[0])
    {
        lstrcpyA (_outputFileName, _workdir);
        lstrcatA (_outputFileName, "llm_input-");
        lstrcatA (_outputFileName, guid);
        lstrcatA (_outputFileName, ".txt");
    }

    // имя выходного файла
    lstrcpyA (_outputFileName, _workdir);
    lstrcatA (_outputFileName, DEFAULT_OUTPUT_FILE_NAME);
    if (guid[0])
    {
        lstrcpyA (_outputFileName, _workdir);
        lstrcatA (_outputFileName, "llm_output-");
        lstrcatA (_outputFileName, guid);
        lstrcatA (_outputFileName, ".txt");
    }

    // командная строка
    lstrcpyA (_commandLineArguments, _executable);
    lstrcatA (_commandLineArguments, " -i \"");
    lstrcatA (_commandLineArguments, _inputFileName);
    lstrcatA (_commandLineArguments, "\" -o \"");
    lstrcatA (_commandLineArguments, _outputFileName);
    lstrcatA (_commandLineArguments, "\"");
}

//====================================================================

// Экспортируемые функции, вызываемые АРМом.
// inputBuffer – передаваемые данные (входные),
// outputBuffer – возвращаемые данные (выходные),
// outputBufferSize – размер выходного буфера (outputBuffer).
// Как правило, размер выходного буфера составляет 32000 байт.
// В ИРБИС64 данные передаются и возвращаются в UTF8.
// Переводы строки стандартные для Windows: \r\n
// Возвращаемое значение: 0 – нормальное завершение;
// любое другое значение – ненормальное.

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
    SecureZeroMemory (outputBuffer, outputBufferSize);

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
    SecureZeroMemory (outputBuffer, outputBufferSize);

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
    SecureZeroMemory (outputBuffer, outputBufferSize);

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
// На сервере, естественно, нас никто не услышит
#pragma comment(linker, "/export:Bleep=_Bleep@12")
int OUR_API Bleep
    (
        const char *inputBuffer,
        char *outputBuffer,
        int outputBufferSize
    )
{
    NOT_USED (inputBuffer);

    SecureZeroMemory (outputBuffer, outputBufferSize);

    Beep (750, 300);
    MessageBeep (0xFFFFFFFF); // A simple beep.
    // If the sound card is not available,
    // the sound is generated using the speaker.

    return 0;
}

//====================================================================

/*

   &uf('+8llmthunk,Show,Всё пропало!')

 */

// Вывод сообщения в стандартном окне.
// Не использовать для форматов, отрабатывающих на сервере.
// Всё тупо зависнет.
#pragma comment(linker, "/export:Show=_Show@12")
int OUR_API Show
    (
        const char *inputBuffer,
        char *outputBuffer,
        int outputBufferSize
    )
{
    SecureZeroMemory (outputBuffer, outputBufferSize);

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
    SecureZeroMemory ((char*) wideBuffer, wideBufferSize);
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

/*
   &uf('+8llmthunk,Run,Далеко ли до Луны?',#,'*****')

   или

   &uf('+8llmthunk,Run,',&uf('6promptaiforimages'))

 */

#pragma comment(linker, "/export:Run=_Run@12")
int OUR_API Run
    (
        const char *inputBuffer,
        char *outputBuffer,
        int outputBufferSize
    )
{
    SecureZeroMemory (outputBuffer, outputBufferSize);

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

    SetupWorkDirectoryAndFiles (inputBuffer);

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

//====================================================================

// запись текста в файл
#pragma comment(linker, "/export:Write=_WriteText@12")
int OUR_API WriteText
    (
        const char *inputBuffer,
        char *outputBuffer,
        int outputBufferSize
    )
{
    SecureZeroMemory (outputBuffer, outputBufferSize);

    const char *comma = lstrchrA (inputBuffer, ',');
    if (!comma)
    {
        return 1;
    }

    char fileName[MAX_PATH];

    SecureZeroMemory (fileName, MAX_PATH);
    lstrcpynA (fileName, inputBuffer, comma - inputBuffer + 1);

    WriteTextData (fileName, comma + 1);

    return 0;
}

// получение текущей директории
#pragma comment(linker, "/export:Cwd=_Cwd@12")
int OUR_API Cwd
    (
        const char *inputBuffer,
        char *outputBuffer,
        int outputBufferSize
    )
{
    NOT_USED (inputBuffer);

    SecureZeroMemory (outputBuffer, outputBufferSize);

    GetCurrentDirectoryA (outputBufferSize, outputBuffer);

    return 0;
}

// получение имени пользователя
#pragma comment(linker, "/export:User=_GetUser@12")
int OUR_API GetUser
    (
        const char *inputBuffer,
        char *outputBuffer,
        int outputBufferSize
    )
{
    NOT_USED (inputBuffer);

    SecureZeroMemory (outputBuffer, outputBufferSize);

    DWORD size = outputBufferSize;
    GetUserNameA (outputBuffer, &size);

    return 0;
}

// получение имени машины
#pragma comment(linker, "/export:Machine=_GetMachine@12")
int OUR_API GetMachine
    (
        const char *inputBuffer,
        char *outputBuffer,
        int outputBufferSize
    )
{
    NOT_USED (inputBuffer);

    SecureZeroMemory (outputBuffer, outputBufferSize);

    DWORD size = outputBufferSize;
    GetComputerNameA (outputBuffer, &size);

    return 0;
}

// получение рабочей папки, в которую мы положим входной и выходной файлы
#pragma comment(linker, "/export:WorkDir=_GetWorkDir@12")
int OUR_API GetWorkDir
    (
        const char *inputBuffer,
        char *outputBuffer,
        int outputBufferSize
    )
{
    SecureZeroMemory (outputBuffer, outputBufferSize);
    SetupWorkDirectoryAndFiles (inputBuffer);
    lstrcpyA (outputBuffer, _workdir);

    return 0;
}

// получение имени исполняемого файла, который мы должны запустить
#pragma comment(linker, "/export:Exe=_GetExe@12")
int OUR_API GetExe
    (
        const char *inputBuffer,
        char *outputBuffer,
        int outputBufferSize
    )
{
    SecureZeroMemory (outputBuffer, outputBufferSize);
    SetupWorkDirectoryAndFiles (inputBuffer);
    lstrcpyA (outputBuffer, _executable);

    return 0;
}

/*

    &uf('+8llmthunk,Process')

 */

// получение имени исполняемого файла, в который загружена наша DLL
#pragma comment(linker, "/export:Process=_GetProcess@12")
int OUR_API GetProcess
    (
        const char *inputBuffer,
        char *outputBuffer,
        int outputBufferSize
    )
{
    NOT_USED (inputBuffer);

    SecureZeroMemory (outputBuffer, outputBufferSize);
    GetModuleFileName (NULL, outputBuffer, outputBufferSize);

    return 0;
}
