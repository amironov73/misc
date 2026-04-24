cmake_minimum_required(VERSION 3.20)
project(LlmSync C)

set(CMAKE_C_STANDARD 11)

add_executable(LlmSync main.c)

# Указываем линковщику вашу точку входа
target_link_options(LlmSync PRIVATE "/ENTRY:wWinMain")

#  Отключаем специфические проверки MSVC
target_compile_options(LlmSync PRIVATE
        /GS-      # Отключает проверку переполнения стека (security checks)
        /Gs100000 # Отключает stack probes
)

# Убираем RTC (Runtime Checks) для всех конфигураций (Debug/Release)
string(REPLACE "/RTC1" "" CMAKE_C_FLAGS_DEBUG "${CMAKE_C_FLAGS_DEBUG}")

# Отключаем стандартные библиотеки
target_link_options(LlmSync PRIVATE
        "/NODEFAULTLIB"
        "/SUBSYSTEM:CONSOLE"
)

# Линкуем системные библиотеки Win32
target_link_libraries(LlmSync kernel32.lib user32.lib)