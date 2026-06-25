using DG.Tweening;
using UnityEngine;
namespace Menu
{

public class Scaler : MonoBehaviour
{
    private Tweener _currentTween;

    public Tweener GetTwinResiz(Vector3 scale, float duration, float delay = 1, float overshoot = 1)
    {
        _currentTween?.Kill();

        return _currentTween = transform.DOScale(scale, duration)
            .SetDelay(delay)
            .SetEase(Ease.OutBack, overshoot: overshoot);
    }

    public void SetInactive(float minScale)
    {
        _currentTween?.Kill();
        gameObject.SetActive(false);
        transform.localScale = Vector3.one * minScale;
    }
}
}