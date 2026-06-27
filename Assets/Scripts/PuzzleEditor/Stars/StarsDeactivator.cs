using System.Collections;
using System.Linq;
using PuzzleEditor.Counter;
using PuzzleEditor.SoundEditor;
using UnityEngine;
using YG;

namespace PuzzleEditor.Stars
{
    public class StarsDeactivator : MonoBehaviour
    {
        private const int MinIndexValue = 0;

        [SerializeField] private AudioClip _audioClip;
        [SerializeField] private Timer _timer;

        private StarIndicator[] _stars;
        private StarsCounter _starsCounter;
        private Voiceover _voiceover;
        private WaitForSeconds _waitForSeconds;
        private float _delay;
        private bool _isPlaying;

        private void Awake()
        {
            _delay = 0.3f;
            _starsCounter = GetComponent<StarsCounter>();
            _voiceover = GetComponent<Voiceover>();
            _waitForSeconds = new WaitForSeconds(_delay);

            _stars = GetComponentsInChildren<StarIndicator>(true)
            .OrderBy(s => s.transform.GetSiblingIndex())
            .ToArray();
        }

        private IEnumerator Start()
        {
            yield return null;

            StartCoroutine(StartCountdown());
        }

        private void OnEnable()
        {
            _timer.HasBegun += OnTimerStarted;
        }

        private void OnDisable()
        {
            _timer.HasBegun -= OnTimerStarted;
        }

        public IEnumerator StartCountdown()
        {
            foreach (var star in _stars)
            {
                yield return _waitForSeconds;

                star.TurnOn();
            }
        }

        public void OnTimerStarted()
        {
            StartCoroutine(DeactivateByTime());
        }

        private IEnumerator DeactivateByTime()
        {
            int minStars = _starsCounter.MinStars;

            for (int i = _stars.Length - 1; i >= minStars; i--)
            {
                yield return new WaitUntil(() =>
                {
                    SaveCurrentStars();

                    int count = _starsCounter.GetCountStars(_timer.CurrentTimeSeconds);
                    return count <= i;
                });

                _stars[i].SetInactive();
                _voiceover.PlayOneShot(_audioClip);
            }
        }

        private void SaveCurrentStars()
        {
            if (_timer.IsRunning == false && _isPlaying == false)
            {
                _isPlaying = true;
                int starsLeft = _stars.Count(s => s.IsActive);
                starsLeft = Mathf.Max(starsLeft, _starsCounter.MinStars);
                SetCountStars(starsLeft);
            }
        }

        private void SetCountStars(int count)
        {
            if (count < 0)
            {
                Debug.LogWarning($"SetCountStars: star count {count} is out of valid range (minimum: {MinIndexValue}). Clamping to {MinIndexValue}.");
                YG2.saves.Stars = MinIndexValue;
                return;
            }

            YG2.saves.Stars = count;
        }
    }
}