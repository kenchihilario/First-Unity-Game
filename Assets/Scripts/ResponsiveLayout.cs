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
        
        // On Desktop (Landscape), match height so UI doesn't become massively zoomed in
        // On Mobile (Portrait), match width so UI doesn't get clipped on the sides
        scaler.matchWidthOrHeight = isLandscape ? 1f : 0f;

        // Optionally tighten up the bottom nav bar for desktop so it doesn't stretch across the entire monitor
        if (navLayout != null)
        {
            navLayout.childForceExpandWidth = false;
            navLayout.childControlWidth = false;
        }
    }
}
