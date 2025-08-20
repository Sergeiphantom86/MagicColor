using UnityEngine;
using YG;

public class MoneyLoader : MonoBehaviour
{
    [SerializeField] private CoinWallet _coinWallet;
    [SerializeField] private CrystalWallet  _crystalWallet;

    private float _duration;

    private void Awake()
    {
        _duration = 1;
    }

    private void Start()
    {
        _coinWallet.AddFunds(YG2.saves.CurrentCoin, _duration);
        _crystalWallet.AddFunds(YG2.saves.CurrentCrystal, _duration);
    }
}