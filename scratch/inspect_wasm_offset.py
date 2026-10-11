import os

wasm_path = r"D:\Projects for PC\Unity\My project\WebGLBuild\Build\WebGLBuild.wasm"

with open(wasm_path, "rb") as f:
    f.seek(0x11e71b6 - 64)
    chunk = f.read(256)
    print("Bytes at 0x11e71b6:")
    print(" ".join(f"{b:02x}" for b in chunk[:64]))
    print(" ".join(f"{b:02x}" for b in chunk[64:128]))

# Also let's check if there is a name section at the end of the wasm file
with open(wasm_path, "rb") as f:
    # Read sections
    f.seek(8) # Skip magic and version
    file_size = os.path.getsize(wasm_path)
    print(f"Total file size: {file_size} (0x{file_size:x})")
