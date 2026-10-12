wasm_path = r"D:\Projects for PC\Unity\My project\WebGLBuild\Build\WebGLBuild.wasm"



with open(wasm_path, "rb") as f:

    f.seek(0x11e71b6)

    data = f.read(118)



print("Hex bytes:")

print(" ".join(f"{b:02x}" for b in data))

