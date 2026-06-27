using Menu.Shop;
using UnityEngine;
using YG;

namespace Wallets.WalletEditor
{
    public class AbilityQuantityLoader : MonoBehaviour
    {
        [SerializeField] private PurchaseButton _purchaseButton;

        private int _balance;
        private BagAbilities _bagAbilities;

        private void Awake()
        {
            _bagAbilities = GetComponent<BagAbilities>();

            if (_bagAbilities == null)
            {
                Debug.LogError("BagAbilities == null");
            }

            OnUpdateBalance(GetBalance());
        }

        private void Start()
        {
            _bagAbilities.Add(GetBalance());
        }

        private void OnEnable()
        {
            _bagAbilities.BagChanged += OnUpdateBalance;
            _purchaseButton.Clicked += OnAdd;
        }

        private void OnDisable()
        {
            _bagAbilities.BagChanged -= OnUpdateBalance;
            _purchaseButton.Clicked -= OnAdd;
        }

        private void OnDestroy()
        {
            SaveToFile();
        }

        private int GetBalance()
        {
            return YG2.saves.QuantityAbilities;
        }

        private void OnUpdateBalance(int balance)
        {
            _balance = balance;
        }

        private void SaveToFile()
        {
            YG2.saves.QuantityAbilities = _balance;
        }

        private void OnAdd()
        {
            if (_bagAbilities != null)
            _bagAbilities.Add();
        }
    }
}