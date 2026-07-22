using System;
using System.Collections;
using PuzzleResources;
using UnityEngine;
using YG;

namespace Menu.Tutorials
{
    public class HintCounter : MonoBehaviour
    {
        [SerializeField] private BlocksContainer _container;
        [SerializeField] private int _requiredCoins = 3000;
        [SerializeField] private float _hintDelaySeconds = 60f;

        private Coroutine _hintCoroutine;
        private WaitForSeconds _waitForSeconds;

        public event Action Worked;

        public event Action Rested;

        private void Awake()
        {
            _waitForSeconds = new WaitForSeconds(_hintDelaySeconds);
        }

        private void OnEnable()
        {
            _container.Destroyed += StartTimer;
            _container.EverythingDestroyed += OnStopTimer;
        }

        private void OnDisable()
        {
            _container.Destroyed -= StartTimer;
            _container.EverythingDestroyed -= OnStopTimer;
        }

        public void StartTimer()
        {
            Rested?.Invoke();

            if (YG2.saves.IsUnlockAbilities && YG2.saves.CurrentCoin >= _requiredCoins)
            {
                ResetTimer();
            }
        }

        public void ResetTimer()
        {
            OnStopTimer();

            _hintCoroutine = StartCoroutine(ShowHintAfterDelay());
        }

        private IEnumerator ShowHintAfterDelay()
        {
            yield return _waitForSeconds;

            Worked?.Invoke();
        }

        private void OnStopTimer()
        {
            if (_hintCoroutine != null)
            {
                StopCoroutine(_hintCoroutine);
                _hintCoroutine = null;
            }
        }
    }
}