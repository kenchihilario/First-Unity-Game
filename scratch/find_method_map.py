import sys
sys.stdout.reconfigure(encoding='utf-8')

with open(r"D:\Projects for PC\Unity\My project\Library\Bee\artifacts\WebGL\il2cppOutput\cpp\Symbols\MethodMap.tsv", "r", encoding="utf-8", errors="ignore") as f:
    for i in range(20):
        parts = f.readline().strip().split('\t')
        print(parts)
