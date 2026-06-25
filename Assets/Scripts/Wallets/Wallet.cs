using System;
using System.Collections;
using Game.SaveEditor;
using UnityEngine;
using Wallets.WalletEditor;

namespace Wallets
{
    public class Wallet : MonoBehaviour
    {
        [SerializeField]
        private bool autoLoadFromSave;

        private long _balance;
        private float _delay;
        private float _callDelay;
        private bool isInitialized;
        private WaitForSeconds _waitFor;
        private IProgressSaver _progressSaver;
        private IProcessTransacter _transacter;

        public event Action<long, string> OnBalanceChanged;

        public long Balance => _balance;

        public string Name => GetType().Name;

        private void Awake()
        {
            _delay = 1.5f;
            _callDelay = 0.1f;
            _waitFor = new WaitForSeconds(_delay);
            _progressSaver = new ProgressSaver();
            _transacter = new ProcessTransacter();
        }

        private void Start()
        {
            if (autoLoadFromSave && _progressSaver.Saves != null)
            {
                LoadFromSave();
            }
        }

        private void OnEnable()
        {
            if (autoLoadFromSave && _progressSaver.Saves != null)
            {
                _progressSaver.SubscribeSDKData(OnYGDataLoaded);
            }
        }

        private void OnDisable()
        {
            if (autoLoadFromSave && _progressSaver.Saves != null)
            {
                _progressSaver.UnsubscribeSDKData(OnYGDataLoaded);
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

                _progressSaver.SaveBalanceAfterPurchase(_balance);
                OnBalanceChanged?.Invoke(_balance, Name);
            }
            else
            {
                Debug.LogWarning(
                    $"������������ �������! ������� ��������� {amount}, �� ������ ����� {_balance}"
                );
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
            if (_progressSaver.Saves == null)
                return;

            if (this is CoinWallet)
            {
                SetInitialBalance(_progressSaver.Saves.CurrentCoin);
            }
            else if (this is CrystalWallet)
            {
                StartCoroutine(Wait(_progressSaver.Saves.CurrentCrystal));
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