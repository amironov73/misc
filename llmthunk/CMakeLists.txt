cmake_minimum_required(VERSION 3.31)
project(llmthunk C)

set(CMAKE_C_STANDARD 90)

add_library(llmthunk SHARED library.c)
add_definitions(-D_CRT_SECURE_NO_WARNINGS)
set(CMAKE_EXE_LINKER_FLAGS /nodefaultlib)
