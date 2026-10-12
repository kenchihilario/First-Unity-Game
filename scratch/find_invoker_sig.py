import re

import sys

sys.stdout.reconfigure(encoding='utf-8')



with open(r"D:\Projects for PC\Unity\My project\Library\Bee\artifacts\WebGL\il2cppOutput\cpp\Il2CppInvokerTable.cpp", "r", encoding="utf-8", errors="ignore") as f:

    content = f.read()



matches = re.findall(r'(\w+Invoker\w+)\s*\([^)]*\)', content)

print(f"Total invoker matches: {len(matches)}")

print("Sample invokers:", matches[:10])

