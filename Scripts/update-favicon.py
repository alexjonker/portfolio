import requests
from io import BytesIO
from pathlib import Path
from PIL import Image, ImageDraw

img_url = "https://github.com/AlexJonker.png?size=256"
img_path = Path(__file__).resolve().parent.parent / "wwwroot/favicon.png"

print(f"Saving github url to {img_path}")

img = Image.open(BytesIO(requests.get(img_url).content)).convert("RGBA")

# Make it a circle
size = min(img.size)
scale = 16
mask = Image.new("L", (size * scale, size * scale), 0)
ImageDraw.Draw(mask).ellipse((0, 0, size * scale - 1, size * scale - 1), fill=255)
mask = mask.resize((size, size), Image.LANCZOS)
img.putalpha(mask)

img.save(img_path, "PNG")