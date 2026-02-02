using UnityEngine;
using YG;

[RequireComponent(typeof(Wallet))]
public class LeaderboardWallet : MonoBehaviour
{
    private const string Suffix = "Wallet";

    private Wallet _wallet;
    private string _leaderboardName;

    private void Awake()
    {
        _wallet = GetComponent<Wallet>();
        _leaderboardName = ConvertName(_wallet.Name);
    }

    private void OnEnable()
    {
        _wallet.OnBalanceChanged += SavePlayerBalance;
    }

    private void OnDisable()
    {
        _wallet.OnBalanceChanged -= SavePlayerBalance;
    }

    private void SavePlayerBalance(long balance, string walletName)
    {
        _leaderboardName = ConvertName(walletName);

        if (string.IsNullOrEmpty(_leaderboardName))
        {
            Debug.LogError($"Ќе удалось преобразовать им€ кошелька: {walletName}");
            return;
        }

        YG2.SetLeaderboard(_leaderboardName, (int)balance);
    }

    private string ConvertName(string original)
    {
        if (string.IsNullOrEmpty(original))
            return "Default";

        if (original.EndsWith(Suffix))
            return original[..^Suffix.Length];

        return original;
    }
}