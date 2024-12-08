using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class HoverHighlightText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Color highlightColor = Color.yellow;
    private Color originalColor;
    public TextMeshProUGUI text;
    public GameObject icon;

    void Start()
    {
        if (text != null)
        {
            originalColor = text.color;
        }
        if (icon != null)
        {
            icon.SetActive(false); // Ensure the icon is initially hidden
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (text != null)
        {
            text.color = highlightColor;
        }
        if (icon != null)
        {
            icon.SetActive(true); // Show the icon on hover
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetHoverState();
    }

    public void ResetHoverState()
    {
        if (text != null)
        {
            text.color = originalColor;
        }
        if (icon != null)
        {
            icon.SetActive(false);
        }
    }
}
