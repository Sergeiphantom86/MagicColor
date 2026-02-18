using UnityEngine;

public class WalletInitializer : MonoBehaviour
{
    [SerializeField] private CoinWallet coinWallet;
    [SerializeField] private CrystalWallet crystalWallet;

    private IProgressSaver _progressSaver;

    private void Awake()
    {
        _progressSaver = new ProgressSaver();
    }

    private void Start()
    {
        if (coinWallet != null)
        {
            long savedCoins = _progressSaver.Saves.CurrentCoin;
        }

        if (crystalWallet != null)
        {
            long savedCrystals = _progressSaver.Saves.CurrentCrystal;
        }
    }
}