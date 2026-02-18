using UnityEngine;

public class ProcessTransacter : IProcessTransacter
{
    public bool ProcessTransaction(long amount, long balance)
    {
        if (amount == 0) return false;

        if (amount < 0)
        {
            if (Mathf.Abs(amount) > balance)
                return false;
        }

        long newBalance = checked(balance + amount);

        if (newBalance != balance)
        {
            return true;
        }

        return false;
    }
}