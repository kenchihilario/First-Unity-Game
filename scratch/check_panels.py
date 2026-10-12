import re



scene_path = r"D:\Projects for PC\Unity\My project\Assets\Scenes\SampleScene.unity"



with open(scene_path, "r", encoding="utf-8") as f:

    content = f.read()



panels = ["LobbyPanel", "CrashPanel", "RoulettePanel", "SlotsPanel", "MinesPanel", "PlinkoPanel", "DicePanel", "BlackjackPanel", "LeaderboardPanel", "Overlays"]



for p in panels:

    matches = list(re.finditer(rf'--- !u!1 &(\d+)\nGameObject:.*?m_Name: {p}\n  m_IsActive: (\d+)', content, re.DOTALL))

    for m in matches:

        print(f"{p:<20} ID: {m.group(1)} Active: {m.group(2)}")

