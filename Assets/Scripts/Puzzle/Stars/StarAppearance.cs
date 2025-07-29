using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class StarAppearance : MonoBehaviour
{
    private Star _star;
    private Image _inactivePart;

    private const float AnimationDuration = 0.5f;
    private const float MinScale = 0.2f;
    private const float Overshoot = 1.5f;

    private Tweener _currentTween;

    private void Awake() 
    {
        _inactivePart = GetComponent<Image>();
        _star = GetComponentInChildren<Star>();

        _inactivePart.enabled = true;
        _star.SetActive(false);
    }

    public void TurnOn(float delay)
    {
        _currentTween?.Kill();
        _star.SetActive(true);
        _star.transform.localScale = Vector3.one * MinScale;

        _currentTween = _star.transform.DOScale(Vector3.one, AnimationDuration)
            .SetDelay(delay)
            .SetEase(Ease.OutBack, overshoot: Overshoot);
    }

    public void SetInactive()
    {
        _currentTween?.Kill();
        _inactivePart.enabled = true;
        _star.SetActive(false);
    }
}