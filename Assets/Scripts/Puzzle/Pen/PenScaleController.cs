using UnityEngine;
using DG.Tweening;

public class PenScaleController : MonoBehaviour
{
    [SerializeField] private float _scaleMultiplier = 1.5f;
    [SerializeField] private float _scaleDuration = 0.3f;
    [SerializeField] private float _returnDuration = 0.2f;

    private Vector3 _originalScale;
    private Tween _currentTween;

    private void Awake()
    {
        _originalScale = transform.localScale;
    }

    public void StartScaleUp()
    {
        ChangeSize(_originalScale * _scaleMultiplier, _scaleDuration);
    }

    public void StartScaleDown()
    {
        ChangeSize(_originalScale, _returnDuration);
    }

    public void Stop()
    {
        ClearTween();
        transform.localScale = _originalScale;
    }

    private void ChangeSize(Vector3 scale, float duration)
    {
        ClearTween();
        _currentTween = transform.DOScale(scale, duration)
            .SetEase(Ease.InOutQuad);
    }

    private void ClearTween()
    {
        if (_currentTween != null && _currentTween.IsActive())
        {
            _currentTween.Kill();
        }
        _currentTween = null;
    }
}