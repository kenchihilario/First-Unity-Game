import sys

sys.stdout.reconfigure(encoding='utf-8')



with open(r"D:\Projects for PC\Unity\My project\Library\Bee\artifacts\WebGL\il2cppOutput\cpp\Symbols\MethodMap.tsv", "r", encoding="utf-8", errors="ignore") as f:

    for line in f:

        parts = line.strip().split('\t')

        if len(parts) >= 3 and parts[2] == "Assembly-CSharp":

            print(f"{parts[0]:<40} {parts[1]}")

