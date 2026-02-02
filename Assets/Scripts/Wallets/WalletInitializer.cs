using UnityEngine;
using YG;

public class WalletInitializer : MonoBehaviour
{
    [SerializeField] private CoinWallet coinWallet;
    [SerializeField] private CrystalWallet crystalWallet;

    private void Start()
    {
        if (coinWallet != null)
        {
            long savedCoins = YG2.saves.CurrentCoin;
            //coinWallet.LoadBalance(savedCoins);
            //Debug.Log($"Coins initialized from save: {savedCoins}");
        }

        if (crystalWallet != null)
        {
            long savedCrystals = YG2.saves.CurrentCrystal;
            //crystalWallet.LoadBalance(savedCrystals);
            //Debug.Log($"Crystals initialized from save: {savedCrystals}");
        }
    }
}