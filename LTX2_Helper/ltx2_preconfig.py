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
                "target_frames": ("INT", {"default": 65, "min": 1, "max": 257, "step": 1}),
            }
        }

    RETURN_TYPES = ("INT", "INT", "INT")
    RETURN_NAMES = ("width", "height", "frames")
    FUNCTION = "calculate"
    CATEGORY = "LTX2/Config"

    def calculate(self, resolution, target_frames):
        # Парсим строку разрешения
        res_part = resolution.split(" (")[0] # Убираем текст в скобках
        width, height = map(int, res_part.split(" x "))

        # Формула для кадров: кратность 8 + 1
        # (x // 8) * 8 + 1 гарантирует, что 64 станет 65, а 60 станет 57 (или 65)
        # Для предсказуемости используем round:
        frames = ((target_frames - 1) // 8) * 8 + 1
        if frames < 1: frames = 1

        return (width, height, frames)

NODE_CLASS_MAPPINGS = {"LTX2_PreConfig": LTX2_PreConfig}
NODE_DISPLAY_NAME_MAPPINGS = {"LTX2_PreConfig": "🚀 LTX2 PreConfig"}
