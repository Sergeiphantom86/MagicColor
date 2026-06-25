using UnityEngine;
namespace Menu
{

public class CarouselData
{
    public CarouselData(ButtonKeeper keeper)
    {
        int count = keeper.Buttons.Length;

        Buttons = new RectTransform[count];
        CanvasGroups = new CanvasGroup[count];
        OriginalPositions = new Vector2[count];

        for (int i = 0; i < count; i++)
        {
            Buttons[i] = keeper.Buttons[i].GetComponent<RectTransform>();
            CanvasGroups[i] = Buttons[i].GetComponent<CanvasGroup>();
            OriginalPositions[i] = Buttons[i].anchoredPosition;
        }
    }

    public RectTransform[] Buttons { get; }

    public CanvasGroup[] CanvasGroups { get; }

    public Vector2[] OriginalPositions { get; }
}
}