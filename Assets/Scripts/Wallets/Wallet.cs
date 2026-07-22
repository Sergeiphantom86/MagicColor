using System;
using System.Collections;
using UnityEngine;
using Wallets.WalletEconomy;
using YG;

namespace Wallets
{
    public abstract class Wallet : MonoBehaviour
    {
        [SerializeField] private bool _autoLoadFromSave;

        private long _balance;
        private float _callDelay;
        private bool _isInitialized;
        private IProcessTransacter _transacter;
        private WaitForSeconds _waitForSeconds;

        public event Action<long, string> BalanceChanged;
        public long Balance => _balance;
        public string Name => GetType().Name;

        protected abstract long LoadBalanceFromSave();
        protected abstract void SaveBalanceToSave(long balance);
        protected virtual float GetLoadDelay() => 0f;

        private void Awake()
        {
            _callDelay = 0.1f;
            _transacter = new ProcessTransacter();
            _waitForSeconds = new WaitForSeconds(GetLoadDelay());
        }

        private void Start()
        {
            if (_autoLoadFromSave)
                LoadFromSave();
        }

        private void OnEnable()
        {
            if (_autoLoadFromSave)
                YG2.onGetSDKData += OnYGDataLoaded;
        }

        private void OnDisable()
        {
            if (_autoLoadFromSave)
                YG2.onGetSDKData -= OnYGDataLoaded;
        }

        public bool SpendFunds(long amount)
        {
            if (CanSpend(amount) == false)
                return false;

            bool success = _transacter.ProcessTransaction(amount, _balance);

            if (success)
            {
                _balance -= amount;
                SaveBalanceToSave(_balance);
                BalanceChanged?.Invoke(_balance, Name);
            }

            return success;
        }

        private void LoadFromSave()
        {
            StartCoroutine(LoadBalanceCoroutine());

            _isInitialized = true;
        }

        private IEnumerator LoadBalanceCoroutine()
        {
            yield return _waitForSeconds;

            long loadedBalance = LoadBalanceFromSave();

            SetInitialBalance(loadedBalance);
        }

        private void SetInitialBalance(long amount)
        {
            _balance = amount;
            BalanceChanged?.Invoke(_balance, Name);
        }

        private bool CanSpend(long amount) => 
            amount > 0 && _balance >= amount;

        private void OnYGDataLoaded()
        {
            if (_autoLoadFromSave && !_isInitialized)
                Invoke(nameof(LoadFromSave), _callDelay);
        }
    }
}