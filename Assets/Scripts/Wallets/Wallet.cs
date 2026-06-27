using System;
using System.Collections;
using UnityEngine;
using Wallets.WalletEditor;
using YG;

namespace Wallets
{
    public class Wallet : MonoBehaviour
    {
        [SerializeField] private bool autoLoadFromSave;

        private long _balance;
        private float _delay;
        private float _callDelay;
        private bool isInitialized;
        private WaitForSeconds _waitFor;
        private IProcessTransacter _transacter;

        public event Action<long, string> OnBalanceChanged;

        public long Balance => _balance;

        public string Name => GetType().Name;

        private void Awake()
        {
            _delay = 1.5f;
            _callDelay = 0.1f;
            _waitFor = new WaitForSeconds(_delay);
            _transacter = new ProcessTransacter();
        }

        private void Start()
        {
            if (autoLoadFromSave)
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

        public bool SpendFunds(long amount)
        {
            if (CanSpend(amount) == false)
                return false;

            bool success = _transacter.ProcessTransaction(amount, _balance);

            if (success)
            {
                _balance -= amount;

                YG2.saves.CurrentCoin = _balance;

                OnBalanceChanged?.Invoke(_balance, Name);
            }
            else
            {
                Debug.LogWarning($"Insufficient: {amount} > {_balance}", this);
            }

            return success;
        }

        private void OnYGDataLoaded()
        {
            if (autoLoadFromSave && isInitialized == false)
            {
                Invoke(nameof(LoadFromSave), _callDelay);
            }
        }

        private void LoadFromSave()
        {

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

        private IEnumerator Wait(long savedCrystals)
        {
            yield return _waitFor;

            SetInitialBalance(savedCrystals);
        }
    }
}