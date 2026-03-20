cmake_minimum_required(VERSION 4.1)
project(llmthunk C)

set(CMAKE_C_STANDARD 11)

add_library(llmthunk SHARED library.c
        path.c
        path.h
        main.c
        llm.h
        string.c
        string.h)
