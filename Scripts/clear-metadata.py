import shutil
import subprocess
import sys
from pathlib import Path

path = img_path = Path(__file__).resolve().parent.parent

file_extensions = [
    "jpg", "jpeg", "png", "gif", "webp", "tif", "tiff", "heic", "heif", "avif",
    "mp4", "m4v", "mov", "avi", "mkv", "webm",
]


if shutil.which("exiftool") is None:
        print("Error: exiftool not installed.")
        exit()

cmd = ["exiftool", "-all=", "-overwrite_original", "-r", "-q", "-progress"]
for ext in file_extensions:
    cmd += ["-ext", ext]

cmd.append(str(path))

result = subprocess.run(cmd)

print("Done.")
