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
    types = []
    func_types = []
    
    while True:
        b = f.read(1)
        if not b: break
        sec_id = b[0]
        size = read_leb128(f)
        pos = f.tell()
        
        if sec_id == 1: # Types
            num_types = read_leb128(f)
            for _ in range(num_types):
                form = f.read(1)[0] # 0x60 = func
                num_params = read_leb128(f)
                params = [f.read(1)[0] for _ in range(num_params)]
                num_results = read_leb128(f)
                results = [f.read(1)[0] for _ in range(num_results)]
                types.append((params, results))
        elif sec_id == 3: # Functions
            num_funcs = read_leb128(f)
            for _ in range(num_funcs):
                func_types.append(read_leb128(f))
        f.seek(pos + size)

type_names = {0x7f: 'i32', 0x7e: 'i64', 0x7d: 'f32', 0x7c: 'f64', 0x70: 'funcref', 0x6f: 'externref'}

for b_idx in [76275, 76562]:
    t_idx = func_types[b_idx]
    p, r = types[t_idx]
    p_str = ", ".join(type_names.get(x, f"0x{x:x}") for x in p)
    r_str = ", ".join(type_names.get(x, f"0x{x:x}") for x in r)
    print(f"Body #{b_idx} has Type #{t_idx}: ({p_str}) -> ({r_str})")
