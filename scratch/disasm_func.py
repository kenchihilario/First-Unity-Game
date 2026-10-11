wasm_path = r"D:\Projects for PC\Unity\My project\WebGLBuild\Build\WebGLBuild.wasm"

with open(wasm_path, "rb") as f:
    f.seek(0x11e71b5)
    # read the whole body of this function
    # LEB128 size
    def read_leb128(f):
        res = 0; shift = 0
        while True:
            b = f.read(1)[0]
            res |= (b & 0x7f) << shift
            if not (b & 0x80): break
            shift += 7
        return res
    
    size = read_leb128(f)
    print(f"Function size: {size}")
    data = f.read(size)
    print("Full bytes:\n" + " ".join(f"{b:02x}" for b in data))

    # Let's find all `call` (0x10) and `call_indirect` (0x11) instructions in this function
    i = 0
    while i < len(data):
        op = data[i]
        if op == 0x10: # call
            # read LEB128 target func
            target = 0; shift = 0
            j = i + 1
            while True:
                b = data[j]
                target |= (b & 0x7f) << shift
                j += 1
                if not (b & 0x80): break
                shift += 7
            print(f"Offset +{i:x}: CALL function {target}")
            i = j
            continue
        elif op == 0x11: # call_indirect
            target = 0; shift = 0
            j = i + 1
            while True:
                b = data[j]
                target |= (b & 0x7f) << shift
                j += 1
                if not (b & 0x80): break
                shift += 7
            table_idx = data[j]
            print(f"Offset +{i:x}: CALL_INDIRECT type {target} table {table_idx}")
            i = j + 1
            continue
        i += 1
