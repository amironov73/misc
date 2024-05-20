@echo off

lms server start
lms load "andrewcanis/c4ai-command-r-v01-GGUF/c4ai-command-r-v01-f16.gguf" --gpu=auto -y
lms status