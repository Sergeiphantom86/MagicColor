using TMPro;
using UnityEngine.UI;

public class Pause 
{
    public void Configure(Button button, HandlerButtonWindowInteraction manager)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => manager.Show("Pause"));
        button.GetComponentInChildren<TMP_Text>().text = "Пауза";
    }
}