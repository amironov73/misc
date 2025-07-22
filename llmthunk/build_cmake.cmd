@echo off

@rem ==========================================
@rem Используется CMake, поставляемый с CLion.
@rem Ничто не менает поменять на любой другой.
@rem ==========================================

mkdir build
cd build
%USERPROFILE%\AppData\Local\Programs\CLion\bin\cmake\win\x64\bin\cmake.exe -A Win32 -D CMAKE_BUILD_TYPE=Release ..
%USERPROFILE%\AppData\Local\Programs\CLion\bin\cmake\win\x64\bin\cmake.exe --build . --config Release
