using UnityEngine;
using YG;

namespace Wallets
{
    [RequireComponent(typeof(Wallet))]

    public class LeaderboardWallet : MonoBehaviour
    {
        private const string Suffix = "Wallet";
        private const string Default = nameof(Default);

        private Wallet _wallet;
        private string _leaderboardName;

        private void Awake()
        {
            _wallet = GetComponent<Wallet>();

            _leaderboardName = ConvertName(_wallet.Name);
        }

        private void OnEnable()
        {
            _wallet.BalanceChanged += OnSavePlayerBalance;
        }

        private void OnDisable()
        {
            _wallet.BalanceChanged -= OnSavePlayerBalance;
        }

        private string ConvertName(string original)
        {
            if (string.IsNullOrEmpty(original))
                return Default;

            if (original.EndsWith(Suffix))
                return original[..^Suffix.Length];

            return original;
        }

        private void OnSavePlayerBalance(long balance, string walletName)
        {
            _leaderboardName = ConvertName(walletName);

            if (_leaderboardName == Default)
            {
                Debug.LogError($"Leaderboard name is not set for wallet: {walletName}", this);
                return;
            }

            YG2.SetLeaderboard(_leaderboardName, (int)balance);
        }
    }
}