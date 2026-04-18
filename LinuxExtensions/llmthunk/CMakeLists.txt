cmake_minimum_required(VERSION 4.1)
project(llmthunk C)

set(CMAKE_C_STANDARD 11)

add_library(llmthunk SHARED library.c
        main.c
        llm.h
)

target_link_libraries(${PROJECT_NAME} uuid)