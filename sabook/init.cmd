@echo off

python -m venv venv
venv\Scripts\pip install -r requirements.txt
venv\Scripts\pip install pylint
venv\Scripts\python -m spacy download ru_core_news_lg
