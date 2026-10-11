import re

invoker_file = r"D:\Projects for PC\Unity\My project\Library\Bee\artifacts\WebGL\il2cppOutput\cpp\Il2CppInvokerTable.cpp"

# We know the body of body #76275 has 1 arg of 8 bytes (i64, e.g. int64_t, uint64_t, double, or struct of 8 bytes)
# and returns an int32 or similar
with open(invoker_file, "r", encoding="utf-8", errors="ignore") as f:
    text = f.read()

# Let's search for functions in Il2CppInvokerTable.cpp with:
# ((Func)methodPointer)((*(int64_t*)args[0]), methodMetadata)
# or similar
matches = re.findall(r'void (RuntimeInvoker_\w+)\s*\([^)]*\)\s*\{([^}]+)\}', text)
print(f"Total invokers parsed: {len(matches)}")

# Filter matches where args[0] is dereferenced and method has 1 param + methodMetadata
candidates = []
for name, body in matches:
    if "methodMetadata" in body and "args[0]" in body and "args[1]" not in body:
        candidates.append((name, body.strip()))

print(f"Candidates with 1 arg: {len(candidates)}")
for name, body in candidates[:10]:
    print(f"--- {name} ---")
    print(body)
