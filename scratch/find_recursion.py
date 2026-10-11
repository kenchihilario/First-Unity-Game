import re
import os

cpp_dir = r"D:\Projects for PC\Unity\My project\Library\Bee\artifacts\WebGL\il2cppOutput\cpp"
files = [os.path.join(cpp_dir, "Assembly-CSharp.cpp"), os.path.join(cpp_dir, "Assembly-CSharp__1.cpp")]

for fpath in files:
    if not os.path.exists(fpath):
        continue
    print(f"Scanning {os.path.basename(fpath)}...")
    with open(fpath, "r", encoding="utf-8", errors="ignore") as f:
        content = f.read()

    # Match function definitions like:
    # IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR void SomeFunc_mXYZ (SomeClass_t* __this, const RuntimeMethod* method)
    # {
    # ...
    # }
    func_pattern = re.compile(r'IL2CPP_EXTERN_C\s+IL2CPP_METHOD_ATTR\s+[^\n]+\s+(\w+)\s*\([^)]*\)\s*\{', re.MULTILINE)
    
    matches = list(func_pattern.finditer(content))
    for i, m in enumerate(matches):
        func_name = m.group(1)
        start_pos = m.end()
        end_pos = matches[i+1].start() if i+1 < len(matches) else len(content)
        body = content[start_pos:end_pos]
        
        # Check if func_name is called inside body
        # Call pattern: func_name(...)
        call_pattern = re.compile(r'\b' + re.escape(func_name) + r'\s*\(')
        calls = call_pattern.findall(body)
        if calls:
            print(f"RECURSIVE FUNCTION DETECTED: {func_name} (calls: {len(calls)})")
            # print first 5 lines of body
            body_lines = body.strip().splitlines()[:10]
            print("\n".join(body_lines))
            print("-" * 50)
