using DG.Tweening;
using UnityEngine;
using YG;

public class StarsController : MonoBehaviour
{
    private float _initialDelay;
    private float _delayBetweenStars;
    private StarIndicator[] _stars;
    private Sequence _animationSequence;

    private void Awake()
    {
        _initialDelay = 0.5f;
        _delayBetweenStars = 0.3f;
        _stars = GetComponentsInChildren<StarIndicator>();

        SetActive(false);
    }

    private void OnEnable()
    {
        ResetAll();
    }

    public void ShowWithAnimation(int activeCount)
    {
        _animationSequence?.Kill();

        activeCount = Mathf.Clamp(activeCount, 0, _stars.Length);

        _animationSequence = DOTween.Sequence();
        _animationSequence.AppendInterval(_initialDelay);

        for (int i = 0; i < activeCount; i++)
        {
            int index = i;
            float delay = i * _delayBetweenStars;

            _animationSequence.InsertCallback(_initialDelay + delay,() => 
            _stars[index].TurnOn());
        }

        YG2.saves.SetCountStars(activeCount);
    }

    public void ShowWithOutAnimation(int activeCount)
    {
        activeCount = Mathf.Clamp(activeCount, 0, _stars.Length);

        for (int i = 0; i < activeCount; i++)
        {
            _stars[i].TurnOn();
        }
    }

    public void SetActive(bool isOn)
    {
        gameObject.SetActive(isOn);
    }

    private void ResetAll()
    {
        foreach (var star in _stars)
        {
            star.SetInactive();
        }
    }
}