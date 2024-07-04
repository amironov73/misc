@echo off

python -m venv venv
venv\Scripts\pip install --extra-index-url https://download.pytorch.org/whl/cu121 torch==2.2.2+cu121
venv\Scripts\pip install transformers accelerate pillow
