using System;
using System.Collections;
using PuzzleResources;
using UnityEngine;
using YG;

namespace Menu.Tutorials
{
    public class HintCounter : MonoBehaviour
    {
        private const float HintDelay = 60;

        [SerializeField] private BlocksContainer _container;

        private Coroutine _hintCoroutine;
        private WaitForSeconds _waitForSeconds;

        public event Action Worked;

        public event Action Rested;

        private void Awake()
        {
            _waitForSeconds = new WaitForSeconds(HintDelay);
        }

        private void OnEnable()
        {
            _container.Destroyed += StartTimer;
            _container.EverythingDestroyed += OnStopTimer;
        }

        private void OnDisable()
        {
            _container.Destroyed -= StartTimer;
            _container.EverythingDestroyed += OnStopTimer;
        }

        public void StartTimer()
        {
            Rested?.Invoke();

            if (YG2.saves.IsUnlockAbilities && YG2.saves.CurrentCoin >= 3000)
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