/* This is an open source non-commercial project. Dear PVS-Studio, please check it.
 * PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com */

#include <windows.h>
#include "resource.h"

#if defined(_MSC_VER)
    #pragma warning(disable: 4068)
    #pragma warning(disable: 4152)
#endif

#pragma ide diagnostic ignored "hicpp-signed-bitwise"
#pragma ide diagnostic ignored "OCUnusedGlobalDeclarationInspection"

#ifndef UNUSED
    #define UNUSED(__x) (void)(__x)
#endif

static HHOOK keyboardHook = NULL;
static HANDLE guardMutex = NULL;
static NOTIFYICONDATA trayData = { 0 };

/* {B837DA0C-4EE5-4F77-8C8A-23DDC290595C} */
static GUID trayGuid =
    {
        0xb837da0c,
        0x4ee5,
        0x4f77,
        0x8c, 0x8a, 0x23, 0xdd,
        0xc2, 0x90, 0x59, 0x5c
    };

static const char CLASS_NAME[] = "CapsSwitchClass";
static HWND mainWindow; /* main window handle */
static HICON icon; /* icon used both for main window and for tray */

/* Message used to notify window about tray icon events */
#define WMAPP_NOTIFYCALLBACK (WM_APP + 1)

/*
static void cancelCapsOnWindow
    (
        HWND hwnd
    )
{
    PostMessage (hwnd, WM_KEYUP, 20, 0);
    PostMessage (hwnd, WM_KEYDOWN, 20, 0);
    PostMessage (hwnd, WM_KEYUP, 20, 0);
}
*/

/* Keyboard hook handler routine */
static LRESULT CALLBACK keyboardHookFunction
    (
        int code,
        WPARAM wParam,
        LPARAM lParam
    )
{
    if (code < 0 || code != HC_ACTION) {
        return CallNextHookEx (keyboardHook, code, wParam, lParam);
    }

    if (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN) {
        KBDLLHOOKSTRUCT *kbDllHookStruct = (KBDLLHOOKSTRUCT *) lParam;

        DWORD scanCode = kbDllHookStruct->scanCode;

        if (scanCode == 54) {
            /* CAPSLOCK = 58 */
            /* Right SHIFT = 54 */
            HWND hWnd = GetForegroundWindow();
            PostMessage
                (
                    hWnd,
                    WM_INPUTLANGCHANGEREQUEST, 
                    INPUTLANGCHANGE_FORWARD, 
                    0
                );
            /* cancelCapsOnWindow(hWnd); */
            return 1;
        }
    }

    return CallNextHookEx(keyboardHook, code, wParam, lParam);
}

/* Exit with error message */
void ErrorExit()
{
    LPVOID lpMessageBuffer;
    DWORD code = GetLastError();

    FormatMessage
        (
            FORMAT_MESSAGE_ALLOCATE_BUFFER |
            FORMAT_MESSAGE_FROM_SYSTEM |
            FORMAT_MESSAGE_IGNORE_INSERTS,
            NULL,
            code,
            MAKELANGID (LANG_NEUTRAL, SUBLANG_DEFAULT),
            (LPTSTR) &lpMessageBuffer,
            0,
            NULL
        );

    MessageBox
        (
            NULL,
            (LPCTSTR) lpMessageBuffer,
            TEXT ("Error"),
            MB_OK
        );

    LocalFree (lpMessageBuffer);
    ExitProcess (code);
}

/* Copy GUID data */
static void copyGuid
    (
        GUID *dest,
        GUID *src
    )
{
    dest->Data1    = src->Data1;
    dest->Data2    = src->Data2;
    dest->Data3    = src->Data3;
    dest->Data4[0] = src->Data4[0];
    dest->Data4[1] = src->Data4[1];
    dest->Data4[2] = src->Data4[2];
    dest->Data4[3] = src->Data4[3];
    dest->Data4[4] = src->Data4[4];
    dest->Data4[5] = src->Data4[5];
    dest->Data4[6] = src->Data4[6];
    dest->Data4[7] = src->Data4[7];
}

/* Create an icon in the tray area */
static BOOL createTrayIcon ()
{
    trayData.cbSize = NOTIFYICONDATA_V1_SIZE;
    trayData.hWnd = mainWindow;
    trayData.uID = 0;
    trayData.hIcon = icon;
    copyGuid (&trayData.guidItem, &trayGuid);
    trayData.szTip[0] = 0;
    trayData.dwState = 0;
    trayData.dwStateMask = 0;
    trayData.szInfo[0] = 0;
    trayData.uCallbackMessage = WMAPP_NOTIFYCALLBACK;
    trayData.uFlags = NIF_ICON | NIF_MESSAGE  | NIF_GUID;

    if (!Shell_NotifyIcon (NIM_ADD, &trayData))
    {
        ErrorExit();
        return FALSE;
    }

    //trayData.uVersion = NOTIFYICON_VERSION_4;
    //Shell_NotifyIcon (NIM_SETVERSION, &trayData);

    return TRUE;
}

/* Delete the icon from the tray */
static void deleteTrayIcon()
{
    trayData.cbSize = NOTIFYICONDATA_V1_SIZE;
    copyGuid (&trayData.guidItem, &trayGuid);
    trayData.uFlags = NIF_GUID;

    Shell_NotifyIcon (NIM_DELETE, &trayData);
}

/* Show the context menu near the tray icon */
static void showContextMenu
    (
        HWND hwnd,
        POINT point
    )
{
    HINSTANCE hInstance = GetModuleHandle (NULL);
    HMENU hMenu = LoadMenu (hInstance, MAKEINTRESOURCE (IDR_TRAYMENU));
    if (hMenu) {
        HMENU hSubMenu = GetSubMenu (hMenu, 0);
        if (hSubMenu)
        {
            /* our window must be foreground before calling
             * TrackPopupMenu or the menu will not disappear
             * when the user clicks away */
            SetForegroundWindow (hwnd);

            /* respect menu drop alignment */
            UINT uFlags = TPM_RIGHTBUTTON;
            TrackPopupMenuEx
                (
                    hSubMenu,
                    uFlags,
                    point.x,
                    point.y,
                    hwnd,
                    NULL
                );
        }
        DestroyMenu(hMenu);
    }
}

/* Handle the tray icon notifications */
static void handleNotify
    (
        HWND hwnd,
        WPARAM wParam,
        LPARAM lParam
    )
{
    POINT point;
    int command = LOWORD (lParam);

    UNUSED (wParam);

    if (command == WM_RBUTTONUP)
    {
        GetCursorPos (&point);
        showContextMenu (hwnd, point);
    }
}

/* Handle the command from the context menu */
static void handleCommand
    (
        HWND hwnd,
        WPARAM wParam,
        LPARAM lParam
    )
{
    WORD command = LOWORD (wParam);

    UNUSED (wParam);
    UNUSED (lParam);

    if (command == IDM_EXIT)
    {
        DestroyWindow (hwnd);
    }
}

/* Window procedure */
static LRESULT CALLBACK windowProc
    (
        HWND hwnd,
        UINT uMsg,
        WPARAM wParam,
        LPARAM lParam
    )
{
    switch (uMsg)
    {
        case WM_DESTROY:
            deleteTrayIcon();
            PostQuitMessage (0);
            return 0;

        case WM_COMMAND:
            handleCommand (hwnd, wParam, lParam);
            return 0;

        case WMAPP_NOTIFYCALLBACK:
            handleNotify (hwnd, wParam, lParam);
            return 0;

        default:
            break;
    }

    return DefWindowProc (hwnd, uMsg, wParam, lParam);
}

/* Register the window class */
static BOOL registerClass
    (
        HINSTANCE hInstance
    )
{
    WNDCLASS wc = { 0 };
    wc.lpfnWndProc   = windowProc;
    wc.hInstance     = hInstance;
    wc.lpszClassName = CLASS_NAME;

    ATOM result = RegisterClass (&wc);

    return result != 0;
}

/* Create the window */
static BOOL createWindow
    (
        HINSTANCE hInstance
    )
{
    DWORD style = WS_CAPTION|WS_SYSMENU;
    DWORD styleEx = 0;

    mainWindow = CreateWindowEx
        (
            styleEx,              /* Optional window styles */
            CLASS_NAME,           /* Window class */
            "CapsSwitch",         /* Window text */
            style,                /* Window style */

            /* Size and position */
            CW_USEDEFAULT,
            CW_USEDEFAULT,
            CW_USEDEFAULT,
            CW_USEDEFAULT,

            NULL,       /* Parent window */
            NULL,       /* Menu */
            hInstance,  /* Instance handle */
            NULL        /* Additional application data */
        );

    if (!mainWindow) {
        goto DONE;
    }

    /* load icon from the resources */
    icon = (HICON) LoadImage
        (
            hInstance,
            MAKEINTRESOURCE (IDI_MAIN_ICON),
            IMAGE_ICON,
            0,
            0,
            LR_DEFAULTCOLOR | LR_DEFAULTSIZE
        );
    SendMessage
        (
            mainWindow,
            WM_SETICON,
            ICON_BIG,
            (LPARAM) icon
        );
    SendMessage
        (
            mainWindow,
            WM_SETICON,
            ICON_SMALL,
            (LPARAM) icon
        );

    DONE:
    return mainWindow != NULL;
}

/* The program entry point */
/* void WINAPI mainCRTStartup () -- console app without runtime */
/* void WINAPI WinMainCRTStartup () -- windowed app without runtime */
void WINAPI WinMainCRTStartup ()
/* int WINAPI WinMain
    (
        HINSTANCE hinstance,
        HINSTANCE hPrevious,
        LPSTR pCmdLine,
        int nCmdShow
    ) */
{
    MSG msg;
    HINSTANCE hinstance = GetModuleHandle (NULL);
    UINT exitCode = 0;

    guardMutex = CreateMutex (NULL, FALSE, "CapsSwitchMutex");
    if (!guardMutex) {
        exitCode = 1;
        goto DONE;
    }

    if (GetLastError() == ERROR_ALREADY_EXISTS) {
        exitCode = 1;
        goto DONE;
    }

    if (!registerClass (hinstance)
        || !createWindow (hinstance)
        || !createTrayIcon ()) {
        exitCode = 1;
        goto DONE;
    }

    /* set the keyboard hook */
    keyboardHook = SetWindowsHookEx
        (
            WH_KEYBOARD_LL,
            keyboardHookFunction,
            NULL,
            0
        );

    if (keyboardHook == NULL) {
        exitCode = 1;
        goto DONE;
    }

    /* pump the message loop */
    while (GetMessage (&msg, NULL, 0, 0) != 0) {
        DispatchMessage(&msg);
    }


DONE:
    if (guardMutex != NULL) {
        CloseHandle (guardMutex);
    }

    if (keyboardHook != NULL) {
        UnhookWindowsHookEx (keyboardHook);
    }

    if (exitCode) {
        MessageBox
            (
                NULL,
                "Some error occurred",
                "CaspSwitch",
                MB_ICONERROR|MB_OK
            );
    }

    ExitProcess (exitCode);
}
