import sys
sys.stdout.reconfigure(encoding='utf-8')

with open(r"D:\Projects for PC\Unity\My project\Library\Bee\artifacts\WebGL\il2cppOutput\cpp\Il2CppInvokerTable.cpp", "r", encoding="utf-8", errors="ignore") as f:
    for i in range(40):
        print(f.readline().strip())
