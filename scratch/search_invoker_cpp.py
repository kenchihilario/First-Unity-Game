import re

import sys

sys.stdout.reconfigure(encoding='utf-8')



with open(r"D:\Projects for PC\Unity\My project\Library\Bee\artifacts\WebGL\il2cppOutput\cpp\Il2CppInvokerTable.cpp", "r", encoding="utf-8", errors="ignore") as f:

    text = f.read()



funcs = re.findall(r'(IL2CPP_EXTERN_C\s+[^\n]+\s+(\w+)\s*\([^)]*\)\s*\{[^}]+\})', text)

print(f"Total functions in Il2CppInvokerTable.cpp: {len(funcs)}")



