@echo off

@rem =================================================
@rem Предполагается, что нижеследующие команды
@rem запускаются в среде, настроенной для компиляторов,
@rem входящих в состав Microsoft Visual Studio.
@rem =================================================

del LlmSync.exe 2> nul

cl /nologo /GS- /TC /O2 /Ob2 /DNDEBUG /D_CRT_SECURE_NO_WARNINGS main.c ^
   /link /NODEFAULTLIB /ENTRY:wWinMain /SUBSYSTEM:CONSOLE /MACHINE:X64 ^
   kernel32.lib user32.lib shell32.lib ^
   /OUT:LlmSync.exe