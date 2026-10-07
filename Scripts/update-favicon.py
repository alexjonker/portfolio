import requests
from pathlib import Path

img_url = "https://github.com/AlexJonker.png"
img_path = Path(__file__).resolve().parent.parent / "wwwroot/favicon.png"

print(f"Saving github url to {img_path}")

img_data = requests.get(img_url).content
with open(img_path, "wb") as handler:
    handler.write(img_data)
