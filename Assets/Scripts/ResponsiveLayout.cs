using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ResponsiveLayout : MonoBehaviour
{
    public CanvasScaler scaler;
    public HorizontalLayoutGroup navLayout;
    
    void Update()
    {
        if (scaler == null) return;
        
        bool isLandscape = Screen.width > Screen.height;
        


        scaler.matchWidthOrHeight = isLandscape ? 1f : 0f;


        if (navLayout != null)
        {
            navLayout.childForceExpandWidth = false;
            navLayout.childControlWidth = false;
        }
    }
}
