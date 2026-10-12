import re



scene_path = r"D:\Projects for PC\Unity\My project\Assets\Scenes\SampleScene.unity"



with open(scene_path, "r", encoding="utf-8") as f:

    content = f.read()



ids = [602314859, 644773935, 691310472, 707638716, 708970178, 747210061, 751785874, 774052688, 782952580, 805916447, 902175239, 1313645073, 1824739263, 1861321442]



for obj_id in ids:

    m = re.search(rf'--- !u!1 &{obj_id}\nGameObject:.*?m_Name: (.*?)\n', content, re.DOTALL)

    if m:

        print(f"ID {obj_id}: {m.group(1)}")

    else:



        m2 = re.search(rf'--- !u!1 &(\d+)\nGameObject:.*?m_Component:.*?fileID: {obj_id}', content, re.DOTALL)

        print(f"ID {obj_id} not directly GameObject, m2: {m2.group(1) if m2 else 'none'}")

