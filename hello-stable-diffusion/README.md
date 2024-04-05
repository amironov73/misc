# Hello, Stable Diffusion!

Simple command line utility generating images from the prompt.

Available arguments:

* **--prompt**      specifies the positive prompt
* **--negative**    specifies the negative prompt,
                    default: "ugly, deformed, disfigured, poor details, bad anatomy"
* **--steps**       number of the generation steps, default value: 50
* **--scale**       CFG scale, default value: 7.5
* **--weights**     weights, default "7.5 | 7.5 | -7.5"
* **--seed**        seed for random number generation, 
                    default value: None (seed choosed automatically)
* **--width**       width of the image, default value: 512
* **--height**      height of the image, default value: 512
* **--model_path**  path for the SD model, default value: "prompthero/openjourney-v4"
* **--num_images**  number of the images to generate, default value: 1
