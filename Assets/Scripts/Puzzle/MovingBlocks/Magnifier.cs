using UnityEngine;
using DG.Tweening;

public class Magnifier : MonoBehaviour
{
    [SerializeField] private float _selectedScale;
    [SerializeField] private float _animationDuration;
    [SerializeField] private EffectsHandler effectsHandler;

    private Vector3 _originalScale;
    private Tween _scaleTween;
    private Transform _transform;

    private void Awake()
    {
        _originalScale = transform.localScale;
        _transform = transform;
    }

    public Tween Select()
    {
        _scaleTween = ChangeSize(_originalScale * _selectedScale);

        return _scaleTween;
    }

    public Tween Deselect()
    {
        _scaleTween = ChangeSize(_originalScale);

        return _scaleTween;
    }

    public Tween ChangeSize(Vector3 scale)
    {
        return _transform.DOScale(scale, _animationDuration);

    }

    private void OnDestroy()
    {
        _scaleTween?.Kill();
    }
}