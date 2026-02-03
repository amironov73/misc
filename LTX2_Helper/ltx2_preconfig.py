class LTX2_PreConfig:
    @classmethod
    def INPUT_TYPES(s):
        return {
            "required": {
                "resolution": ([
                                   "768 x 512 (Landscape)",
                                   "512 x 768 (Portrait)",
                                   "864 x 480 (Cinematic)",
                                   "544 x 960 (Vertical)",
                                   "1024 x 576 (HD-ish)"
                               ],),
                "fps": ("INT", {"default": 24, "min": 1, "max": 60, "step": 1}),
                "seconds": ("INT", {"default": 3, "min": 1, "max": 20, "step": 1}),
            }
        }

    RETURN_TYPES = ("INT", "INT", "INT", "INT", "INT")
    RETURN_NAMES = ("width", "height", "FPS", "seconds", "frames")
    FUNCTION = "calculate"
    CATEGORY = "LTX2/Config"

    def calculate(self, resolution, fps, seconds):
        # Парсим строку разрешения
        res_part = resolution.split(" (")[0] # Убираем текст в скобках
        width, height = map(int, res_part.split(" x "))
        frames = (seconds * fps // 8) * 8 + 1
        if frames < 1: frames = 1

        return (width, height, fps, frames)

NODE_CLASS_MAPPINGS = {"LTX2_PreConfig": LTX2_PreConfig}
NODE_DISPLAY_NAME_MAPPINGS = {"LTX2_PreConfig": "🚀 LTX2 PreConfig"}
