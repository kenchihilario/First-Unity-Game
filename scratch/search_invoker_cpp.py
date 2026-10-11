import re
import sys
sys.stdout.reconfigure(encoding='utf-8')

# In Il2CppInvokerTable.cpp, let's see how invokers are defined.
# Let's find functions in Il2CppInvokerTable.cpp that have 5 or 6 parameter casts:
with open(r"D:\Projects for PC\Unity\My project\Library\Bee\artifacts\WebGL\il2cppOutput\cpp\Il2CppInvokerTable.cpp", "r", encoding="utf-8", errors="ignore") as f:
    text = f.read()

# An invoker definition typically looks like:
# IL2CPP_EXTERN_C void* Invoker_XYZ (Il2CppMethodPointer method, const RuntimeMethod* methodInfo, void* obj, void** args, void* ret)
# Or similar
funcs = re.findall(r'(IL2CPP_EXTERN_C\s+[^\n]+\s+(\w+)\s*\([^)]*\)\s*\{[^}]+\})', text)
print(f"Total functions in Il2CppInvokerTable.cpp: {len(funcs)}")

# Let's count how many functions are defined before our target index if we know the order.
