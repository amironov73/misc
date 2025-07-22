@echo off

@rem ==========================================
@rem Предполагается, что нижеследующие команды
@rem выполняются в среде, в которой настроены
@rem пути к компилятору и линкеру Visual Studio.
@rem ==========================================

cl /LD /D_CRT_SECURE_NO_WARNINGS /machine:X86 /DWIN32 /D_WINDOWS /O2 /Ob2 /DNDEBUG /MD library.c kernel32.lib /link /INCREMENTAL:NO kernel32.lib user32.lib