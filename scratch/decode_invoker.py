wasm_path = r"D:\Projects for PC\Unity\My project\WebGLBuild\Build\WebGLBuild.wasm"

# Let's decode every instruction of body 76562 (at 0x11e71b6, length 118)
with open(wasm_path, "rb") as f:
    f.seek(0x11e71b6)
    data = f.read(118)

# The instructions start after locals:
# 01 01 7f -> 1 local declaration of count 1, type i32
# So params: let's see type 5 or func type

print("Hex bytes:")
print(" ".join(f"{b:02x}" for b in data))
