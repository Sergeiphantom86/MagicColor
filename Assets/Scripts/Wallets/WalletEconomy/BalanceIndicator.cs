using TMPro;
using UnityEngine;

namespace Wallets.WalletEconomy
{
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
            _bag.BagChanged += OnShow;
        }

        private void OnDisable()
        {
            _bag.BagChanged -= OnShow;
        }

        private void OnShow(int balance)
        {
            _textMeshProUGUI.text = balance.ToString();
        }
    }
}