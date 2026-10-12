import re



invoker_file = r"D:\Projects for PC\Unity\My project\Library\Bee\artifacts\WebGL\il2cppOutput\cpp\Il2CppInvokerTable.cpp"



with open(invoker_file, "r", encoding="utf-8", errors="ignore") as f:

    text = f.read()



matches = re.findall(r'void (RuntimeInvoker_\w+)\s*\([^)]*\)\s*\{([^}]+)\}', text)

print(f"Total invokers parsed: {len(matches)}")



candidates = []

for name, body in matches:

    if "methodMetadata" in body and "args[0]" in body and "args[1]" not in body:

        candidates.append((name, body.strip()))



print(f"Candidates with 1 arg: {len(candidates)}")

for name, body in candidates[:10]:

    print(f"--- {name} ---")

    print(body)

