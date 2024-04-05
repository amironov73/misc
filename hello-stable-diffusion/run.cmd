@echo off

del image_*.* 2> nul
venv\Scripts\python app.py --num_images 4 --steps 30 --width 512 --height 680 --prompt "photorealistic portrait of a pretty walking down the street, high quality, detailed, 4k, masterpiece"