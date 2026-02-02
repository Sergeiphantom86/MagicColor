using DG.Tweening;
using System.Collections;
using System.Linq;
using UnityEngine;
using YG;

public class StarsDeactivator : MonoBehaviour
{
    [SerializeField] private AudioClip _audioClip;
    [SerializeField] private Timer _timer;
    
    private StarIndicator[] _stars;
    private StarsCounter _starsCounter;
    private Voiceover _voiceover;
    private WaitForSeconds _waitForSeconds;
    private float _delay;

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
        
        StartCoroutine( StartCountdown());
    }

    private void OnEnable()
    {
        _timer.HasBegun += StopCountdown;
    }

    private void OnDisable()
    {
        _timer.HasBegun -= StopCountdown;
    }

    public IEnumerator StartCountdown()
    {
        foreach (var star in _stars)
        {
            yield return _waitForSeconds;
         
            star.TurnOn();
        }
    }

    public void StopCountdown()
    {
        StartCoroutine(DeactivateByTime());
    }

    private IEnumerator DeactivateByTime()
    {
        float timePerStar = _starsCounter.GetTimePerStar();
        int currentStar = _stars.Length - 1;

        while (currentStar >= _starsCounter.MinStars && _timer.IsRunning)
        {
            yield return new WaitForSeconds(timePerStar);

            _voiceover.Play(_audioClip);
            _stars[currentStar].SetInactive();
            currentStar--;
        }

        TrySaveQuantity(currentStar);

        SaveMinQuantity();
    }

    private void TrySaveQuantity(int currentStar)
    {
        if (_timer.IsRunning == false)
        {
            int starsLeft = currentStar + 1;
            Save(starsLeft);
        }
    }

    private void SaveMinQuantity()
    {
        if (_timer.IsRunning)
        {
            Save(_starsCounter.MinStars);
        }
    }

    private void Save(int stars)
    {
        YG2.saves.SetCountStars(stars);
    }
}