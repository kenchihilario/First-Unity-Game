import re



invoker_file = r"D:\Projects for PC\Unity\My project\Library\Bee\artifacts\WebGL\il2cppOutput\cpp\Il2CppInvokerTable.cpp"



with open(invoker_file, "r", encoding="utf-8", errors="ignore") as f:

    content = f.read()



p_start = content.find("g_Il2CppInvokerPointers[13921] =")

table_text = content[p_start:]

invoker_names = re.findall(r'(RuntimeInvoker_\w+)', table_text)



print(f"Total invokers in table: {len(invoker_names)}")



