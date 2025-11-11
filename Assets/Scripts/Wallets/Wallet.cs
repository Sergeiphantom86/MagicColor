using System;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    private long _balance;

    public long Balance => _balance;
    public float Duration { get; private set; }

    public event Action<long, string> OnBalanceChanged;
    public event Action<long, string> OnSpendSuccess;

    public void AddFunds(long amount, float duration)
    {
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
            OnSpendSuccess?.Invoke(amount, GetType().Name);
        }
        else
        {
            Debug.LogWarning($"Insufficient funds! Trying to spend {amount}, but balance is {_balance}");
        }

        return success;
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

        try
        {
            _balance = GetNewBalance(amount);

            OnBalanceChanged?.Invoke(_balance, GetType().Name);

            return true;
        }
        catch (OverflowException)
        {
            return false;
        }
    }

    private long GetNewBalance(long amount)
    {
        return checked(_balance + amount);
    }
}