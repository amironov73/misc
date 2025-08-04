@echo off

@rem =================================================
@rem Предполагается, что нижеследующие команды
@rem запускаются в среде, настроенной для компиляторов,
@rem входящих в состав Microsoft Visual Studio.
@rem =================================================

del llmthunk.dll 2> nul

cl /nologo /GS- /Zl /TC /LD /D_CRT_SECURE_NO_WARNINGS ^
   /machine:X86 /DWIN32 /D_WINDOWS /O2 /Ob2 /DNDEBUG library.c ^
   /link /INCREMENTAL:NO /ENTRY:DllMain kernel32.lib user32.lib ^
   advapi32.lib shell32.lib shlwapi.lib /OUT:llmthunk.dll

dumpbin /exports llmthunk.dll
