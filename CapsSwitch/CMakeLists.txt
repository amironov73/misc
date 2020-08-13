cmake_minimum_required(VERSION 2.8)
project(CapsSwitch C)

set(CMAKE_C_STANDARD 90)

add_executable(CapsSwitch WIN32 CapsSwitch.c CapsSwitch.rc resource.h)
add_definitions(-D_CRT_SECURE_NO_WARNINGS)
target_compile_options(${PROJECT_NAME} PRIVATE "/UTF-8 /permissive- /Wall")
# add_compile_options(/UTF-8 /permissive- /Wall)
# set(CMAKE_MSVC_RUNTIME_LIBRARY MultiThreaded)
set(CMAKE_EXE_LINKER_FLAGS /nodefaultlib)