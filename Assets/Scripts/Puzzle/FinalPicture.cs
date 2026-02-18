using DG.Tweening;
using UnityEngine;

public class FinalPicture : MonoBehaviour
{
    [SerializeField] private float _moveYDuration;
    [SerializeField] private float _targetYPosition;
    [SerializeField] private float _targetZPosition;
    [SerializeField] private float _scaleDuration;
    [SerializeField] private float _scaleMultiplier;
    [SerializeField] private Activator _activator;
    [SerializeField] private AudioClip _clip;
    [SerializeField] private Canvas _canvas;

    private Vector3 _targetScale;
    private Sequence _currentSequence;
    private Voiceover _voiceover;
    private ZoomChanger _zoomChanger;
    private IActivatable _activatable;

    private void Awake()
    {
        _voiceover = GetComponent<Voiceover>();
        _zoomChanger = new ZoomChanger();

        _activatable = _activator;

        if (_zoomChanger.IsMobileWithTallScreen() == false)
        {
            _scaleMultiplier = 0.5f;
        }

        _targetScale = Vector3.one * _scaleMultiplier;
    }

    private void OnEnable()
    {
        _activator.OnPuzzleComplete += Demonstrate;
        _activator.OnApproach += ZoomIn;
    }

    private void OnDisable()
    {
        _activator.OnPuzzleComplete -= Demonstrate;
        _activator.OnApproach -= ZoomIn;

        StopCurrentAnimation();
    }

    private void ZoomIn(float time)
    {
        StopCurrentAnimation();

        _voiceover.PlayOneShot(_clip);

        _currentSequence = DOTween.Sequence();

        GetCreatedSequence(time)
            .SetEase(Ease.InElastic)
            .OnComplete(() =>
        {
            _activatable.Deactivate();
            _currentSequence = null;
        });
    }

    private void Demonstrate()
    {
        StopCurrentAnimation();

        _voiceover.Stop();

        _currentSequence = DOTween.Sequence();

        GetCreatedSequence(_moveYDuration);
    }

    private Sequence GetCreatedSequence(float duration)
    {
        _currentSequence
           .Join(transform.DOLocalMoveY(_targetYPosition, duration).SetEase(Ease.OutBack))
           .Join(transform.DOLocalMoveZ(_targetZPosition, duration).SetEase(Ease.OutBack))
           .Join(transform.DOScale(_targetScale, duration).SetEase(Ease.OutBack));

        return _currentSequence;
    }

    private void StopCurrentAnimation()
    {
        if (_currentSequence != null && _currentSequence.IsActive())
        {
            _currentSequence.Kill();
            _currentSequence = null;
        }
    }

    private void OnDestroy()
    {
        StopCurrentAnimation();
    }
}