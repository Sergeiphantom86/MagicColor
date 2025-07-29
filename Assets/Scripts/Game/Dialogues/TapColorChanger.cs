using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class TapColorChanger : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text textToChange;
    [SerializeField] private Color[] colors;
    private int currentIndex = 0;

    public void OnPointerClick(PointerEventData eventData)
    {
        ChangeTextColor();
    }

    private void ChangeTextColor()
    {
        if (colors.Length == 0) return;

        currentIndex = (currentIndex + 1) % colors.Length;
        textToChange.color = colors[currentIndex];
    }
}