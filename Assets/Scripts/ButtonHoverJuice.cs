using UnityEngine;
using UnityEngine.EventSystems;
public class ButtonHoverJuice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 originalScale;
    public float hoverScale = 1.05f;
    public float clickScale = 0.95f;
    public float speed = 15f;
    private Vector3 targetScale;
    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * speed);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * hoverScale;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        targetScale = originalScale * clickScale;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        targetScale = originalScale * hoverScale;
    }
}
