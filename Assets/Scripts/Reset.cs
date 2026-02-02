using UnityEngine;
using UnityEngine.UI;
using YG;

public class Reset : MonoBehaviour
{
   private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void Start()
    {
        _button.onClick.AddListener(TurnOn);
    }

    private void TurnOn()
    {
        YG2.SetDefaultSaves();
        YG2.SaveProgress();
    }
}