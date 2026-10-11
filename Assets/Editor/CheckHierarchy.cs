using UnityEngine;
using UnityEditor;
public class CheckHierarchy
{
    [MenuItem("Casino/Legacy & Dev Tools/Debug Hierarchy", priority = 100)]
    public static void Run()
    {
        Debug.Log("--- SCENE HIERARCHY ---");
        foreach (GameObject go in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            PrintNode(go, 0);
        }
        Debug.Log("--- END SCENE HIERARCHY ---");
    }
    static void PrintNode(GameObject go, int depth)
    {
        string indent = new string('-', depth * 2);
        string comps = "";
        foreach (var c in go.GetComponents<Component>())
        {
            if (c != null) comps += c.GetType().Name + ", ";
        }
        Debug.Log($"{indent}> {go.name} [{comps}]");
        foreach (Transform child in go.transform)
        {
            PrintNode(child.gameObject, depth + 1);
        }
    }
}
