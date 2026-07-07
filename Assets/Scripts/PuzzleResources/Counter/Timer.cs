using System;
using System.Collections;
using Game.Exit;
using PuzzleResources.Stars;
using TMPro;
using UnityEngine;

namespace PuzzleResources.Counter
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private string _timeFormat = "mm':'ss";
        [SerializeField] private BlocksContainer _blocksContainer;
        [SerializeField] private StarsController _starCounter;
        [SerializeField] private PauseMenu _pauseMenu;

        private float _value;
        private bool _isRunning;
        private TimeSpan _span;
        private float _delayCompensation;
        private WaitForSeconds _waitCompensation;
        private WaitForSeconds _blockDelayWait;

        public event Action HasBegun;

        public int CurrentTimeSeconds { get; private set; }
        public TMP_Text TimerText => _timerText;

        public bool IsRunning => _isRunning;

        private void Awake()
        {
            _delayCompensation = 0.1f;
            _waitCompensation = new WaitForSeconds(_delayCompensation);
            _blockDelayWait = new WaitForSeconds(_blocksContainer.DelayTime - _delayCompensation);
        }

        private void Update()
        {
            if (_isRunning && _pauseMenu.IsPaused == false)
            {
                _value += Time.unscaledDeltaTime;

                CurrentTimeSeconds = (int)_value;

                _span = TimeSpan.FromSeconds(_value);

                _timerText.text = _span.ToString(_timeFormat);
            }
        }

        private IEnumerator Start()
        {
            yield return _waitCompensation;

            yield return _blockDelayWait;

            StartTimer();

            HasBegun?.Invoke();
        }

        private void OnEnable()
        {
            _blocksContainer.EverythingDestroyed += StopAndSave;
        }

        private void OnDisable()
        {
            _blocksContainer.EverythingDestroyed -= StopAndSave;
        }

        public void StartTimer()
        {
            if (_isRunning)
            return;

            _isRunning = true;
            _value = 0f;
        }

        public void StopAndSave()
        {
            if (_starCounter == null)
            {
                Debug.LogError("StarCounter not found!");
                return;
            }

            Stop();

            _starCounter.SavePlayerTime(_value);
        }

        public void Stop()
        {
            if (_isRunning == false)
            return;

            _isRunning = false;
        }
    }
}