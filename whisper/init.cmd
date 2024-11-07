@echo off

call environment.cmd

rem python.exe -m pip install --extra-index-url https://download.pytorch.org/whl/cu121 torch==2.2.2+cu121
python.exe -m pip install torch==2.2.2
python.exe -m pip install transformers accelerate pillow pydub
python.exe -m pip install numpy==1.26.4
