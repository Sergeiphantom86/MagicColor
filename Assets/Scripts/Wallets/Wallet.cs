using System;
using System.Collections;
using UnityEngine;
using YG;

public class Wallet : MonoBehaviour
{
    [SerializeField] private bool autoLoadFromSave = true;

    private long _balance;
    private bool isInitialized = false;
    private float _delay;
    private WaitForSeconds _waitFor;

    public long Balance => _balance;
    public float Duration { get; private set; }

    public string Name => GetType().Name;

    public event Action<long, string> OnBalanceChanged;
    public event Action<long, string> OnSpendSuccess;

    private void Awake()
    {
        _delay = 1.5f;
        _waitFor = new WaitForSeconds(_delay);
    }

    private void Start()
    {
        if (autoLoadFromSave && YG2.saves != null)
        {
            LoadFromSave();
        }
    }

    private void OnEnable()
    {
        if (autoLoadFromSave)
        {
            YG2.onGetSDKData += OnYGDataLoaded;
        }
    }

    private void OnDisable()
    {
        if (autoLoadFromSave)
        {
            YG2.onGetSDKData -= OnYGDataLoaded;
        }
    }

    public void AddFunds(long amount, float duration)
    {
        if (amount <= 0) return;

        if (ProcessTransaction(amount))
        {
            Duration = duration;
        }
    }

    public bool SpendFunds(long amount)
    {
        if (CanSpend(amount) == false) return false;

        bool success = ProcessTransaction(-amount);

        if (success)
        {
            OnSpendSuccess?.Invoke(amount, Name);
        }
        else
        {
            Debug.LogWarning($"Insufficient funds! Trying to spend {amount}, but balance is {_balance}");
        }

        return success;
    }

    private void OnYGDataLoaded()
    {
        if (autoLoadFromSave && !isInitialized)
        {
            Invoke(nameof(LoadFromSave), 0.1f);
        }
    }

    private void LoadFromSave()
    {
        if (YG2.saves == null) return;

        if (this is CoinWallet)
        {
            SetInitialBalance(YG2.saves.CurrentCoin);
        }
        else if (this is CrystalWallet)
        {
            StartCoroutine(Wait(YG2.saves.CurrentCrystal));
        }

        isInitialized = true;
    }

    private void SetInitialBalance(long amount)
    {
        _balance = amount;
        OnBalanceChanged?.Invoke(_balance, Name);
    }

    private bool CanSpend(long amount)
    {
        return amount > 0 && _balance >= amount;
    }

    private bool ProcessTransaction(long amount)
    {
        if (amount == 0)
            return false;

        if (amount < 0)
        {
            if (Math.Abs(amount) > _balance)
                return false;
        }

        long newBalance = checked(_balance + amount);

        if (newBalance != _balance)
        {
            _balance = newBalance;

            OnBalanceChanged?.Invoke(_balance, Name);

            return true;
        }

        return false;
    }

    private IEnumerator Wait(long savedCrystals)
    {
        yield return _waitFor;

        SetInitialBalance(savedCrystals);
    }
}