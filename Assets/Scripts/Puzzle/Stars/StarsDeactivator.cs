using System.Collections;
using System.Linq;
using UnityEngine;

public class StarsDeactivator : MonoBehaviour
{
    [SerializeField] private AudioClip _audioClip;
    [SerializeField] private Timer _timer;
    
    private StarIndicator[] _stars;
    private StarsCounter _starsCounter;
    private Voiceover _voiceover;
    private WaitForSeconds _waitForSeconds;
    private float _delay;
    private IProgressSaver _progressSaver;
    private Coroutine _coroutine;
    private bool _isPlaying;

    private void Awake()
    {
        _delay = 0.3f;
        _starsCounter = GetComponent<StarsCounter>();
        _voiceover = GetComponent<Voiceover>();
        _waitForSeconds = new WaitForSeconds(_delay);
        _progressSaver = new ProgressSaver();

        _stars = GetComponentsInChildren<StarIndicator>(true)
            .OrderBy(s => s.transform.GetSiblingIndex())
            .ToArray();
    }

    private IEnumerator Start()
    {
        yield return null;
        
        StartCoroutine( StartCountdown());
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
        _coroutine = StartCoroutine(DeactivateByTime());
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
            _progressSaver.SetCountStars(starsLeft);

            _coroutine = null;
        }
    }
}