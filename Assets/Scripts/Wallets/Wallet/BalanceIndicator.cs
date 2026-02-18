using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class BalanceIndicator : MonoBehaviour
{
    private TextMeshProUGUI _textMeshProUGUI;
    private Bag _bag;

    private void Awake()
    {
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        _bag = GetComponent<Bag>();
    }

    private void OnEnable()
    {
        _bag.OnBagChanged += Show;
    }

    private void OnDisable()
    {
        _bag.OnBagChanged -= Show;
    }

    private void Show(int balance)
    {
        _textMeshProUGUI.text = balance.ToString();
    }
}