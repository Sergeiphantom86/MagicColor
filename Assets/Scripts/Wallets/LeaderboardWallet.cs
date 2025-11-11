using YG;
using UnityEngine;

[RequireComponent(typeof(Wallet))]
public class LeaderboardWallet : MonoBehaviour
{
    private Wallet _wallet;
    private string _leaderboardName;

    private void Awake()
    {
        _wallet = GetComponent<Wallet>();
    }

    private void OnEnable() 
    {
        _wallet.OnBalanceChanged += SavePlayerBalance;
    } 
    private void OnDisable()
    {
        _wallet.OnBalanceChanged -= SavePlayerBalance;
    }

    private void Start()
    {
        LoadLeaderboard();
    }

    private void SavePlayerBalance(long balance, string name)
    {
        if (balance > 0 && string.IsNullOrEmpty(name))
        {
            _leaderboardName = name;

            YG2.SetLeaderboard(_leaderboardName, (int)balance);
            LoadLeaderboard();
        }
    }

    private void LoadLeaderboard() => 
        YG2.GetLeaderboard(_leaderboardName);
}