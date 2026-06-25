using UnityEngine;
using Wallets;
namespace Menu.Shop
{

public class ShoppingWallet : MonoBehaviour
{
    [SerializeField] private Wallet _wallet;
    private PurchaseButton _purchaseButton;

    private void Awake()
    {
        _purchaseButton = GetComponent<PurchaseButton>();
    }

    private void OnEnable()
    {
        _purchaseButton.CoinPurchased += SpendFunds;
    }

    private void OnDisable()
    {
        _purchaseButton.CoinPurchased -= SpendFunds;
    }

    private void SpendFunds(long pay)
    {
        if (_wallet.SpendFunds(pay) == false) 
            return;
        
        _purchaseButton.Click();
    }
}
}