import os

import re



cpp_dir = r"D:\Projects for PC\Unity\My project\Library\Bee\artifacts\WebGL\il2cppOutput\cpp"



codegen_files = []

with open(os.path.join(cpp_dir, "Il2CppCodeRegistration.cpp"), "r", encoding="utf-8") as f:

    for line in f:

        m = re.search(r'\(&g_(\w+)_CodeGenModule\)', line)

        if m:

            codegen_files.append(m.group(1))



total_methods = 0

target_method_idx = 76562



for mod in codegen_files:

    c_name = f"{mod}_CodeGen.c"

    c_path = os.path.join(cpp_dir, c_name)

    if not os.path.exists(c_path):

        continue

    with open(c_path, "r", encoding="utf-8", errors="ignore") as fp:

        txt = fp.read()

    m_count = re.search(r's_methodPointers\[(\d+)\]', txt)

    if m_count:

        count = int(m_count.group(1))

        if total_methods <= target_method_idx < total_methods + count:

            local_idx = target_method_idx - total_methods

            print(f"FOUND IN {mod}! Local idx: {local_idx}")

            m_array = re.search(r's_methodPointers\[\d+\]\s*=\s*\{(.*?)\};', txt, re.DOTALL)

            if m_array:

                ptrs = [p.strip() for p in m_array.group(1).split(',') if p.strip()]

                print(f"Method symbol: {ptrs[local_idx]}")

        total_methods += count



print(f"Total methods across all modules: {total_methods}")

