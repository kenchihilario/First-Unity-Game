import re

# Let's read Il2CppInvokerTable.cpp and extract all function definitions:
invoker_file = r"D:\Projects for PC\Unity\My project\Library\Bee\artifacts\WebGL\il2cppOutput\cpp\Il2CppInvokerTable.cpp"

with open(invoker_file, "r", encoding="utf-8", errors="ignore") as f:
    lines = f.readlines()

# Find all function signatures
funcs = []
for idx, line in enumerate(lines):
    if line.startswith("void RuntimeInvoker_") or line.startswith("static void RuntimeInvoker_") or line.startswith("IL2CPP_EXTERN_C void RuntimeInvoker_"):
        funcs.append((idx + 1, line.strip()))

print(f"Total RuntimeInvoker definitions found: {len(funcs)}")
if funcs:
    print("First 5:", funcs[:5])
    print("Last 5:", funcs[-5:])
