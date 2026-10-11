import os

wasm_path = r"D:\Projects for PC\Unity\My project\WebGLBuild\Build\WebGLBuild.wasm"

def read_leb128(f):
    result = 0
    shift = 0
    while True:
        b = f.read(1)[0]
        result |= (b & 0x7f) << shift
        if not (b & 0x80):
            break
        shift += 7
    return result

with open(wasm_path, "rb") as f:
    f.seek(8)
    
    # Let's find sec 2 (imports) and sec 10 (code)
    num_func_imports = 0
    code_sec_pos = 0
    code_sec_size = 0
    
    while True:
        b = f.read(1)
        if not b:
            break
        sec_id = b[0]
        size = read_leb128(f)
        pos = f.tell()
        
        if sec_id == 2: # imports
            count = read_leb128(f)
            for _ in range(count):
                mod_len = read_leb128(f)
                f.read(mod_len)
                name_len = read_leb128(f)
                f.read(name_len)
                kind = f.read(1)[0]
                if kind == 0: # func import
                    num_func_imports += 1
                    read_leb128(f)
                elif kind == 1: # table
                    f.read(1); read_leb128(f); read_leb128(f) # approx
                elif kind == 2: # memory
                    f.read(1); read_leb128(f) # approx
                elif kind == 3: # global
                    f.read(2)
        elif sec_id == 10:
            code_sec_pos = pos
            code_sec_size = size
            
        f.seek(pos + size)

print(f"Num func imports: {num_func_imports}")
# Function 77414 in wasm:
target_func_idx = 77414
target_code_idx = target_func_idx - num_func_imports
print(f"Target code idx in section 10: {target_code_idx}")

# Now let's traverse Section 10 to find target_code_idx
with open(wasm_path, "rb") as f:
    f.seek(code_sec_pos)
    num_bodies = read_leb128(f)
    print(f"Total function bodies in Sec 10: {num_bodies}")
    
    for i in range(num_bodies):
        func_body_start = f.tell()
        body_size = read_leb128(f)
        body_data_pos = f.tell()
        if i == target_code_idx or func_body_start <= 0x11e71b6 < (body_data_pos + body_size):
            print(f"\nFunction #{num_func_imports + i} (body #{i}):")
            print(f"  Starts at file offset: 0x{func_body_start:x}")
            print(f"  Body size: {body_size} bytes")
            print(f"  Body range: 0x{body_data_pos:x} to 0x{body_data_pos + body_size:x}")
            # read instructions
            f.seek(body_data_pos)
            body_bytes = f.read(body_size)
            print("  Bytes:", " ".join(f"{x:02x}" for x in body_bytes[:60]))
            
        f.seek(body_data_pos + body_size)
