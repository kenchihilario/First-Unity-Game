using UnityEngine;
using UnityEditor;
public class CheckSize
{
    [MenuItem("Casino/Legacy & Dev Tools/Check Size", priority = 100)]
    public static void Run()
    {
        var p = GameObject.Find("LobbyPanel").GetComponent<RectTransform>();
        Debug.Log($"LobbyPanel Rect: {p.rect}, anchors: {p.anchorMin} to {p.anchorMax}, offset: {p.offsetMin} to {p.offsetMax}, scale: {p.localScale}, pos: {p.anchoredPosition}");
    }
}
