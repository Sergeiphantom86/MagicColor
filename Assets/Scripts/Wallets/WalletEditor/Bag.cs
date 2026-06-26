using System;
using UnityEngine;

namespace Wallets.WalletEditor
{
    public class Bag : MonoBehaviour
    {
        private int _balance;

        public event Action<int> BagChanged;

        private void Start()
        {
            BagChanged?.Invoke(_balance);
        }

        public void Add(int amount = 1)
        {
            if (amount <= 0)
                return;

            _balance += amount;

            BagChanged?.Invoke(_balance);
        }

        public bool TryApply(int amount = 1)
        {
            if (amount <= 0 || _balance < amount)
                return false;

            if (ProcessTransaction(amount, _balance) == false)
                return false;

            return true;
        }

        public void Use(int amount = 1)
        {
            if (amount <= 0 || _balance < amount)
                return;

            _balance -= amount;

            BagChanged?.Invoke(_balance);
        }

        private bool ProcessTransaction(long amount, long balance)
        {
            if (amount == 0)
                return false;

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
}