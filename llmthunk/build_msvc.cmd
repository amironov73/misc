@echo off

@rem =================================================
@rem ��������������, ��� ������������� �������
@rem ����������� � �����, ����������� ��� ������������,
@rem �������� � ������ Microsoft Visual Studio.
@rem =================================================

cl /nologo /GS- /Zl /Oi /TC /LD /D_CRT_SECURE_NO_WARNINGS /machine:X86 /DWIN32 /D_WINDOWS /O2 /Ob2 /DNDEBUG library.c /link /INCREMENTAL:NO /ENTRY:DllMain kernel32.lib user32.lib shell32.lib /OUT:llmthunk.dll
dumpbin /exports llmthunk.dll
