using System;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    public event Action<long, string> OnBalanceChanged;

    private long _balance;

    public long Balance => _balance;

    public float Duration { get; private set; }

    public void AddFunds(long amount, float duration)
    {
        ProcessTransaction(amount, TransactionType.Deposit);
        Duration = duration;
    }

    public bool TrySpendFunds(int amount)
    {
        return ProcessTransaction(-amount, TransactionType.Withdrawal).Success;
    }

   

    private TransactionResult ProcessTransaction(long amount, TransactionType type)
    {
        if (amount == 0)
            return new TransactionResult(false, "Zero amount", type, _balance);

        if (amount < 0 && Math.Abs(amount) > _balance)
            return new TransactionResult(false, "Insufficient funds", type, _balance);

        try
        {
            long newBalance = checked(_balance + amount);

            if (newBalance < 0)
                return new TransactionResult(false, "Negative balance", type, _balance);

            _balance = newBalance;
            OnBalanceChanged?.Invoke(_balance, GetName());
            return new TransactionResult(true, string.Empty, type, _balance);
        }
        catch (OverflowException)
        {
            return new TransactionResult(false, "Overflow error", type, _balance);
        }
    }

    private string GetName()
    {
        if (this is CoinWallet)
        {
            return "Coin";
        }

        return "Crystal";
    }
}

public struct TransactionResult
{
    public readonly bool Success;
    public readonly string Message;
    public readonly TransactionType Type;
    public readonly long NewBalance;

    public TransactionResult(bool success, string message, TransactionType type, long newBalance)
    {
        Success = success;
        Message = message;
        Type = type;
        NewBalance = newBalance;
    }
}

public enum TransactionType
{
    Deposit,
    Withdrawal
}