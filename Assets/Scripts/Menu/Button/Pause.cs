using TMPro;
using UnityEngine.UI;

public class Pause 
{
    public void Configure(Button button, HandlerButtonWindowInteraction manager)
    {
        button.onClick.RemoveAllListeners();
        button.GetComponentInChildren<TMP_Text>().text = "Пауза";
    }
}