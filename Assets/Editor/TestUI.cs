using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
public class TestUI
{
    [MenuItem("Casino/Test UI")]
    public static void Run()
    {
        GameObject c = new GameObject("TestCanvas", typeof(RectTransform));
        Canvas can = c.AddComponent<Canvas>();
        can.renderMode = RenderMode.ScreenSpaceOverlay;
        c.AddComponent<CanvasScaler>();
        c.AddComponent<GraphicRaycaster>();
        GameObject p = new GameObject("TestPanel", typeof(RectTransform));
        p.transform.SetParent(c.transform, false);
        Image i = p.AddComponent<Image>();
        i.color = Color.red;
        var r = p.GetComponent<RectTransform>();
        r.anchorMin = Vector2.zero;
        r.anchorMax = Vector2.one;
        r.offsetMin = Vector2.zero;
        r.offsetMax = Vector2.zero;
        c.layer = 5;
        p.layer = 5;
        Debug.Log("Test UI Created. Is it visible?");
    }
}
