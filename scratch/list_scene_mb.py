import re

import os



scene_path = r"D:\Projects for PC\Unity\My project\Assets\Scenes\SampleScene.unity"



with open(scene_path, "r", encoding="utf-8") as f:

    content = f.read()



guid_to_file = {}

for root, dirs, files in os.walk(r"D:\Projects for PC\Unity\My project\Assets"):

    for file in files:

        if file.endswith(".meta"):

            meta_path = os.path.join(root, file)

            with open(meta_path, "r", encoding="utf-8", errors="ignore") as mf:

                txt = mf.read()

                m = re.search(r'guid:\s*([0-9a-f]+)', txt)

                if m:

                    guid_to_file[m.group(1)] = file[:-5]



go_names = {}

for m in re.finditer(r'--- !u!1 &(\d+)\nGameObject:.*?m_Name: (.*?)\n', content, re.DOTALL):

    go_names[m.group(1)] = m.group(2)



custom_scripts = []

for block in content.split("--- !u!114 &"):

    if not block.strip(): continue

    m_go = re.search(r'm_GameObject: \{fileID: (\d+)\}', block)

    m_guid = re.search(r'm_Script: \{fileID: [-\d]+, guid: ([0-9a-f]+)', block)

    if m_go and m_guid:

        go_id = m_go.group(1)

        guid = m_guid.group(1)

        sname = guid_to_file.get(guid, None)

        if sname:

            goname = go_names.get(go_id, "Unknown")

            custom_scripts.append((goname, sname))



for goname, sname in set(custom_scripts):

    print(f"GameObject: {goname:<25} Script: {sname}")

