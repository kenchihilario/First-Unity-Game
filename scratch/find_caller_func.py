wasm_path = r"D:\Projects for PC\Unity\My project\WebGLBuild\Build\WebGLBuild.wasm"

def read_leb128(f):
    result = 0; shift = 0
    while True:
        b = f.read(1)[0]
        result |= (b & 0x7f) << shift
        if not (b & 0x80): break
        shift += 7
    return result

with open(wasm_path, "rb") as f:
    f.seek(8)
    num_func_imports = 417
    code_sec_pos = 0x8bce5
    f.seek(code_sec_pos)
    num_bodies = read_leb128(f)
    
    target_offset = 0x11e33e2
    for i in range(num_bodies):
        start = f.tell()
        size = read_leb128(f)
        body_start = f.tell()
        body_end = body_start + size
        if start <= target_offset < body_end:
            func_idx = num_func_imports + i
            print(f"Target 0x{target_offset:x} is inside function #{func_idx} (body #{i}):")
            print(f"  Range: 0x{start:x} to 0x{body_end:x} (size {size})")
            f.seek(body_start)
            data = f.read(size)
            print("  Bytes: " + " ".join(f"{x:02x}" for x in data[:60]))
            break
        f.seek(body_end)
