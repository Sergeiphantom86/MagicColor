using DG.Tweening;
using UnityEngine;

public class SpawnDropAnimation : MonoBehaviour
{
    [SerializeField] private float _startYOffset;
    [SerializeField] private float _duration;
    [SerializeField] private Ease _ease = Ease.OutCubic;

    private Tween _tween;
    private Vector3 _targetWorldPosition;
    private ColorableObject _targetColor;
    private Collider _collider;

    public float Duration => _duration;

    private void Awake()
    {
        _targetColor = GetComponent<ColorableObject>();
        _collider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        _tween.Play();
    }

    public void Create(Effecter effecter)
    {
        SetTargetPosition();

        SetStartPosition();

        _tween?.Kill();
        _collider.enabled = true;
        _tween = transform
            .DOMove(_targetWorldPosition, _duration)
            .OnComplete(() =>
            {
                effecter.CreateEffect();
                effecter.CraeteParticles(transform.position, Quaternion.identity, 0.5f);
                
                _targetColor.SetAlpha(0.3f);
            })
            .SetEase(_ease)
            .Pause();
    }

    private void SetStartPosition()
    {
        transform.position = _targetWorldPosition + Vector3.up * _startYOffset;
    }

    private void SetTargetPosition()
    {
        _targetWorldPosition = transform.position;
    }
}