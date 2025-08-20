using TMPro;
using UnityEngine;

public class Icon : MonoBehaviour
{
    private TextMeshProUGUI _textMeshProUGUI;

    private void Awake()
    {
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }

    public void SetRank(string rank)
    {
        _textMeshProUGUI.text = rank;
    }
}