wasm_path = r"D:\Projects for PC\Unity\My project\WebGLBuild\Build\WebGLBuild.wasm"



with open(wasm_path, "rb") as f:

    f.seek(0x11e33e2 - 32)

    data = f.read(128)

    print("Bytes around 0x11e33e2:\n" + " ".join(f"{b:02x}" for b in data))

