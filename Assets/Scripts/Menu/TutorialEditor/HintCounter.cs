using System;
using System.Collections;
using PuzzleEditor;
using UnityEngine;
using YG;

namespace Menu.TutorialEditor
{
    public class HintCounter : MonoBehaviour
    {
        [SerializeField] private BlocksContainer _container;

        private Coroutine _hintCoroutine;
        private float _hintDelay;

        public event Action Worked;

        public event Action Rested;

        private void Awake()
        {
            _hintDelay = 60;
        }

        private void OnEnable()
        {
            _container.Destroyed += StartTimer;
            _container.EverythDestroyed += OnStopTimer;
        }

        private void OnDisable()
        {
            _container.Destroyed -= StartTimer;
            _container.EverythDestroyed += OnStopTimer;
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
            yield return new WaitForSeconds(_hintDelay);

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