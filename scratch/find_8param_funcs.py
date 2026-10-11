import re
import os

cpp_dir = r"D:\Projects for PC\Unity\My project\Library\Bee\artifacts\WebGL\il2cppOutput\cpp"

# We want to find functions that have 8 parameters (7 commas in param list)
for f in os.listdir(cpp_dir):
    if not (f.startswith("Assembly-CSharp") or f.startswith("UnityEngine") or f.startswith("Unity.")):
        continue
    if not f.endswith(".cpp"):
        continue
    fpath = os.path.join(cpp_dir, f)
    with open(fpath, "r", encoding="utf-8", errors="ignore") as fp:
        for line_num, line in enumerate(fp):
            if "IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR" in line and "(" in line and ")" in line:
                # count commas inside outer parentheses
                m = re.search(r'\((.*)\)', line)
                if m:
                    params = m.group(1).split(',')
                    if len(params) == 8:
                        if f.startswith("Assembly-CSharp"):
                            print(f"{f}:{line_num+1} -> {line.strip()[:120]}")
