using DG.Tweening;
using UnityEngine;
using YG;

public class StarsController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _delayBetweenStars = 0.3f;
    [SerializeField] private float _initialDelay = 0.5f;

    private StarAppearance[] _stars;
    private Sequence _animationSequence;

    private void Awake()
    {
        _stars = GetComponentsInChildren<StarAppearance>();

        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        ResetAllStars();
    }

    public void ShowStars(int activeCount)
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
            _stars[index].TurnOn(0.1f));
        }

        YG2.saves.SetCountStars(activeCount);
    }

    private void ResetAllStars()
    {
        foreach (var star in _stars)
        {
            star.SetInactive();
        }
    }

    public void SetActive(bool isOn)
    {
        gameObject.SetActive(isOn);
    }
}