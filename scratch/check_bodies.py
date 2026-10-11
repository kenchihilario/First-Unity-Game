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
    code_sec_pos = 0x8bce5
    f.seek(code_sec_pos)
    num_bodies = read_leb128(f)
    
    # We want to know where the invokers start and end.
    # In Il2CppInvokerTable.o, it was compiled as one object file.
    # Let's find which bodies around 76562 look like RuntimeInvokers.
    # RuntimeInvokers usually have call_indirect (0x11) and are relatively small.
    target_idx = 76562
    f.seek(code_sec_pos)
    read_leb128(f)
    
    for i in range(target_idx - 5, target_idx + 6):
        # We need to seek to i. Let's do a fast forward:
        pass

# Let's fast forward to target_idx
with open(wasm_path, "rb") as f:
    f.seek(code_sec_pos)
    read_leb128(f)
    
    for i in range(num_bodies):
        pos = f.tell()
        size = read_leb128(f)
        body_pos = f.tell()
        if i >= 76555 and i <= 76565:
            f.seek(body_pos)
            data = f.read(size)
            print(f"Body #{i} (size {size}): " + " ".join(f"{b:02x}" for b in data[:30]))
        f.seek(body_pos + size)
        if i > 76565:
            break
