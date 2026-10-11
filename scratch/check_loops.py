import os
import re

scripts_dir = r"D:\Projects for PC\Unity\My project\Assets\Scripts"

for fname in os.listdir(scripts_dir):
    if not fname.endswith(".cs"): continue
    fpath = os.path.join(scripts_dir, fname)
    with open(fpath, "r", encoding="utf-8") as f:
        content = f.read()

    # Find while loops
    whiles = re.findall(r'(\bwhile\s*\([^)]*\)\s*\{?[^}]*\}?)', content)
    if whiles:
        print(f"=== {fname} WHILE LOOPS ===")
        for w in whiles:
            print(w.strip()[:100])

    # Find events and delegates
    events = re.findall(r'(\b\w+\s*\?\.Invoke\s*\([^)]*\))', content)
    if events:
        print(f"=== {fname} INVOKES ===")
        for ev in events:
            print(ev)
