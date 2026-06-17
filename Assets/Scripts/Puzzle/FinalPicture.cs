using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalPicture : MonoBehaviour
{
    [SerializeField] private float _moveYDuration;
    [SerializeField] private float _targetYPosition;
    [SerializeField] private float _targetZPosition;
    [SerializeField] private float _scaleDuration;
    [SerializeField] private float _scaleMultiplier;
    [SerializeField] private Activator _activator;
    [SerializeField] private AudioClip _clip;

    private Vector3 _targetScale;
    private Sequence _currentSequence;
    private Voiceover _voiceover;
    private ZoomChanger _zoomChanger;
    private IActivatable _activatable;

    private void Awake()
    {
        _voiceover = GetComponent<Voiceover>();
        _zoomChanger = new ZoomChanger();

        if (_activator == null)
        {
            Debug.LogError($"{nameof(FinalPicture)}: Activator is not assigned!", this);
            enabled = false;
            return;
        }

        if (_zoomChanger.IsMobileWithTallScreen())
        {
            if (SceneManager.GetActiveScene().name == "Tutorial")
            {
                _scaleMultiplier = 0.28f;
                _targetYPosition = 7.5f;
            }
            else
            {
                _scaleMultiplier = 0.17f;
            }
        }

        _activatable = _activator;
        _targetScale = Vector3.one * _scaleMultiplier;
    }

    private void OnEnable()
    {
        if (_activator == null) 
            return;

        _activator.PuzzleCompleted += Demonstrate;
        _activator.Approached += ZoomIn;
    }

    private void OnDisable()
    {
        if (_activator != null)
        {
            _activator.PuzzleCompleted -= Demonstrate;
            _activator.Approached -= ZoomIn;
        }

        StopCurrentAnimation();
    }

    private void ZoomIn(float time)
    {
        StopCurrentAnimation();

        if (_voiceover != null && _clip != null)
        {
            _voiceover.PlayOneShot(_clip);
        }

        _currentSequence = DOTween.Sequence();

        GetCreatedSequence(time)
            .SetEase(Ease.InElastic)
            .OnComplete(() =>
            {
                _activatable?.Deactivate();
                _currentSequence = null;
            });
    }

    private void Demonstrate()
    {
        StopCurrentAnimation();

        if (_voiceover != null)
        {
            _voiceover.Stop();
        }

        _currentSequence = DOTween.Sequence();
        GetCreatedSequence(_moveYDuration);
    }

    private Sequence GetCreatedSequence(float duration)
    {
        if (_currentSequence == null)
        {
            Debug.LogWarning("Sequence is null!");
            return null;
        }

        if (duration <= 0f)
        {
            Debug.LogWarning("Duration must be greater than 0");
            duration = 0.01f;
        }

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