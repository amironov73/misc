#include <stdio.h>
#include <windows.h>

// Выводим сообщение в стандартный поток ошибок
void PrintError
    (
        HANDLE hStdErr,
        LPCWSTR msg
    )
{
    if (hStdErr == INVALID_HANDLE_VALUE || hStdErr == NULL) {
        return;
    }

    // Пишем строку в поток ошибок
    DWORD written;
    WriteConsoleW
        (
            hStdErr,
            msg,
            (DWORD) lstrlenW (msg),
            &written,
            NULL
        );

    // Перенос строки
    WriteConsoleW
        (
            hStdErr,
            L"\r\n",
            2,
            &written,
            NULL
        );
}

// Выводим сообщение о системной ошибке в стандартный поток ошибок
void PrintLastWin32Error
    (
        HANDLE hStdErr
    )
{
    if (hStdErr == INVALID_HANDLE_VALUE || hStdErr == NULL) {
        return;
    }

    LPWSTR errorText = NULL;
    DWORD errorCode = GetLastError();

    FormatMessageW
    (
        FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
        NULL,
        errorCode,
        MAKELANGID (LANG_ENGLISH, SUBLANG_DEFAULT),
        (LPWSTR) &errorText,
        0,
        NULL
    );

    if (errorText)
    {
        DWORD written;
        WriteConsoleW (hStdErr, errorText, (DWORD) lstrlenW (errorText), &written, NULL);
        LocalFree (errorText); // Важно освободить память, выделенную FormatMessage
        // Перенос строки
        WriteConsoleW (hStdErr, L"\r\n", 2, &written, NULL);
    }
}

// Точка входа в программу
int WINAPI wWinMain
    (
        HINSTANCE hInstance,
        HINSTANCE hPrevInstance,
        PWSTR pCmdLine,
        int nCmdShow
    )
{
    (void) hInstance;
    (void) hPrevInstance;
    (void) nCmdShow;

    // Путь к исполняемому файлу, который хотим запустить
    LPWSTR lpApplicationName = L"LlmCall.exe";

    // Структуры для запуска процесса
    STARTUPINFOW si;
    PROCESS_INFORMATION pi;

    // Настройка структур для запуска процесса
    SecureZeroMemory (&si, sizeof (si));
    si.cb = sizeof (si);
    SecureZeroMemory (&pi, sizeof (pi));

    // Получаем стандартные потоки ввода-вывода (для перенаправления)
    HANDLE hInput  = GetStdHandle (STD_INPUT_HANDLE);
    HANDLE hStdOut = GetStdHandle (STD_OUTPUT_HANDLE);
    HANDLE hStdErr = GetStdHandle (STD_ERROR_HANDLE);

    // Убеждаемся, что они наследуемые (на всякий случай)
    SetHandleInformation (hInput,  HANDLE_FLAG_INHERIT, HANDLE_FLAG_INHERIT);
    SetHandleInformation (hStdOut, HANDLE_FLAG_INHERIT, HANDLE_FLAG_INHERIT);
    SetHandleInformation (hStdErr, HANDLE_FLAG_INHERIT, HANDLE_FLAG_INHERIT);

    si.dwFlags = STARTF_USESTDHANDLES;
    si.hStdInput = hInput;
    si.hStdOutput = hStdOut;
    si.hStdError = hStdErr;

    // Запуск процесса
    if (!CreateProcessW (
        lpApplicationName, // Имя модуля
        pCmdLine,          // Командная строка
        NULL,              // Дескриптор процесса не наследуется
        NULL,              // Дескриптор потока не наследуется
        TRUE,              // Наследование дескрипторов
        0,                 // Флаги создания
        NULL,              // Окружение родителя
        NULL,              // Текущий каталог родителя
        &si,               // Указатель на STARTUPINFO
        &pi                // Указатель на PROCESS_INFORMATION
    ))
    {
        // Ошибка запуска
        PrintError (hStdErr, L"Error: Could not create process.");
        PrintLastWin32Error (hStdErr);
        return (int) GetLastError();
    }

    //  Ожидание завершения
    DWORD waitResult = WaitForSingleObject (pi.hProcess, INFINITE);
    if (waitResult == WAIT_FAILED) {
        // Функция не смогла выполнить ожидание
        PrintError (hStdErr, L"Error: Could not wait the process.");
        PrintLastWin32Error (hStdErr);
        return (int) GetLastError();
    }

    // Получение кода возврата
    DWORD exitCode = 0;
    GetExitCodeProcess (pi.hProcess, &exitCode);

    // Очистка ресурсов
    CloseHandle (pi.hProcess);
    CloseHandle (pi.hThread);

    return (int) exitCode;
}
