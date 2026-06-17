using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TouchVisualizer : MonoBehaviour
{
    private float _delay;
    private float _duration;
    private float _overshoot;
    private Vector3 _targetScale;
    private int _scaleMultiplier;
    private int _colorMultiplier;
    private Sequence _sequence;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _delay = 0.1f;
        _overshoot = 5;
        _duration = 0.6f;
        _colorMultiplier = 2;
        _scaleMultiplier = 30;
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _spriteRenderer.material.color *= _colorMultiplier;
        _targetScale = Vector3.one * _scaleMultiplier;
    }

    private void OnDisable()
    {
        _sequence?.Kill();
    }

    public void SetPosition(Vector3 position)
    {
        position.y += 0.5f;

        transform.position = position;
    }

    private void Start()
    {
        Play();
    }

    private void Play()
    {
        _sequence = DOTween.Sequence();

        _sequence.Join(transform.DOScale(_targetScale, _duration));

        if (_spriteRenderer != null)
        {
            _sequence.Join(_spriteRenderer.material.DOFade(0f, _duration));
        }

        _sequence
            .SetDelay(_delay)
            .SetEase(Ease.InOutQuad, _overshoot)
            .SetLoops(-1, LoopType.Restart);
    }

    public void TurnOff()
    {
        _spriteRenderer.enabled = false;
        _sequence.Kill();
    }
}