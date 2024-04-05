import torch as th
import random
from diffusers import DiffusionPipeline
from typing import Optional

import argparse

parser = argparse.ArgumentParser()
parser.add_argument("--prompt", type=str, default="photorealistic portrait of a beautiful girl walking outdoors, high quality, detailed, 4k",
                    help="use '|' as the delimiter to compose separate sentences.")
parser.add_argument("--negative", type=str, default="ugly, deformed, disfigured, poor details, bad anatomy")
parser.add_argument("--steps", type=int, default=50)
parser.add_argument("--scale", type=float, default=7.5)
parser.add_argument("--weights", type=str, default="7.5 | 7.5 | -7.5")
parser.add_argument("--seed", type=Optional[int], default=None)
parser.add_argument("--width", type=int, default=512)
parser.add_argument("--height", type=int, default=512)
parser.add_argument("--model_path", type=str, default="prompthero/openjourney-v4")
parser.add_argument("--num_images", type=int, default=1)
args = parser.parse_args()

has_cuda = th.cuda.is_available()
device = th.device('cpu' if not has_cuda else 'cuda')

prompt = args.prompt
scale = args.scale
steps = args.steps

pipe = DiffusionPipeline.from_pretrained(
    args.model_path,
    torch_dtype=th.float16,
#    use_safetensors=True,
    custom_pipeline="composable_stable_diffusion",
).to(device)

pipe.safety_checker = None

generator = th.Generator(device)
if args.seed:
    generator = generator.manual_seed(args.seed)
else:
    generator = generator.manual_seed(random.randint(1, 1234567890))

for i in range(args.num_images):
    image = pipe(prompt, negative_prompt=args.negative,
                 guidance_scale=scale, num_inference_steps=steps,
                 weights=args.weights, generator=generator,
                 height=args.height, width=args.width,
                 ).images[0]
    image_name = f"image_{i+1}.jpg"
    with open(image_name, "w") as f:
        image.save(f, format="JPEG")
