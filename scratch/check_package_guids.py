import os
import re

missing = {
  "474bcb49853aa07438625e644c072ee6",
  "dc42784cf147c0c48a680349fa168899",
  "172515602e62fb746b5d573b38a5fe58",
  "4e29b1a8efbd4b44bb3f3716e73f07ff",
  "67db9e8f0e2ae9c40bc1e2b64352a6b4",
  "a79441f348de89743a2939f4d699eac1",
  "01614664b831546d2ae94a42149d80ac",
  "fe87c0e1cc204ed48ad3b37840f39efc",
  "5f7201a12d95ffc409449d95f23cf332",
  "d199490a83bb2b844b9695cbf13b01ef",
  "31a19414c41e5ae4aae2af33fee712f6",
  "0cd44c1031e13a943bb63640046fad76",
  "76c392e42b5098c458856cdf6ecaaaa1",
  "1aa08ab6e0800fa44ae55d278d1423e3",
  "30649d3a9faa99c48a7b1166b86bf2a0",
  "cfabb0440166ab443bba8876756fdfa9",
  "fc1515ab76e54f068e2f2207940fab32",
  "8a8695521f0d02e499659fee002a26c2"
}

pkg_dir = r"D:\Projects for PC\Unity\My project\Library\PackageCache"
found = {}
for root, dirs, files in os.walk(pkg_dir):
    for f in files:
        if f.endswith(".meta"):
            fpath = os.path.join(root, f)
            with open(fpath, "r", encoding="utf-8", errors="ignore") as fp:
                txt = fp.read()
                for mg in missing:
                    if mg in txt:
                        found[mg] = os.path.relpath(fpath, pkg_dir)

print(f"Found {len(found)} in PackageCache:")
for mg, p in found.items():
    print(f"  {mg}: {p}")

not_found = missing - set(found.keys())
print(f"Still missing {len(not_found)}:")
for mg in not_found:
    print(f"  {mg}")
