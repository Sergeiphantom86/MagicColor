using UnityEngine;
using YG;
using YG.Utils.LB;

[RequireComponent(typeof(Wallet))]
public class LeaderboardWallet : MonoBehaviour
{
    private const float RequestCoolDown = 1.5f;

    [SerializeField] private Icon _icon;
    
    private Wallet _wallet;
    private string _leaderboardName;
    private float _lastRequestTime;

    private void Awake()
    {
        _wallet = GetComponent<Wallet>();
    }

    private void OnEnable() 
    {
        _wallet.OnBalanceChanged += SavePlayerBalance;
        YG2.onGetLeaderboard += OnLeaderboardLoaded;
    } 
    private void OnDisable()
    {
        _wallet.OnBalanceChanged -= SavePlayerBalance;
        YG2.onGetLeaderboard -= OnLeaderboardLoaded;
    }

    private void Start()
    {
        LoadLeaderboard();
    }

    public void SavePlayerBalance(long balance, string name)
    {
        _leaderboardName = name;
  
        if (balance > 0)
        {
            YG2.SetLeaderboard(_leaderboardName, (int)Mathf.Clamp(balance, 1, int.MaxValue));
            LoadLeaderboard();
        }
    }

    public void LoadPlayerRank()
    {
        if (Time.time - _lastRequestTime < RequestCoolDown) return;

        _lastRequestTime = Time.time;
        YG2.GetLeaderboard(_leaderboardName, 1, 0, "");
    }

    private void OnLeaderboardLoaded(LBData data)
    {
        //LoadPlayerRank();

        //if (data.technoName != _leaderboardName) return;

        //int newRank = -1;

        //if (data.currentPlayer != null)
        //{
        //    newRank = data.currentPlayer.rank;
        //    Debug.Log($"Player rank updated: #{newRank}");
        //}

        //if (newRank != _currentRank)
        //{
        //    _currentRank = newRank;
        //    _icon.SetRank($"{data.currentPlayer.rank}");
        //}

        //_icon.SetRank($"{data.currentPlayer.rank}");
    }

    private void LoadLeaderboard() => 
        YG2.GetLeaderboard(_leaderboardName);
}