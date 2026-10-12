import os

import re



valid_guids = set()

for root, dirs, files in os.walk(r"D:\Projects for PC\Unity\My project"):

    if "Library" in root or "Temp" in root: continue

    for f in files:

        if f.endswith(".meta"):

            fpath = os.path.join(root, f)

            with open(fpath, "r", encoding="utf-8", errors="ignore") as fp:

                txt = fp.read()

                m = re.search(r'guid:\s*([0-9a-f]+)', txt)

                if m:

                    valid_guids.add(m.group(1))



scene_path = r"D:\Projects for PC\Unity\My project\Assets\Scenes\SampleScene.unity"

with open(scene_path, "r", encoding="utf-8") as fp:

    scene_text = fp.read()



missing_guids = set()

for m in re.finditer(r'm_Script: \{fileID: [-\d]+, guid: ([0-9a-f]+)', scene_text):

    g = m.group(1)

    if g not in valid_guids:

        missing_guids.add(g)



print(f"Total missing script GUIDs in scene: {len(missing_guids)}")

for mg in missing_guids:

    print("  Missing GUID:", mg)

