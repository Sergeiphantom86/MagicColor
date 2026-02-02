using DG.Tweening;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class FinalPicture : MonoBehaviour
{
    [SerializeField] private float _moveYDuration;
    [SerializeField] private float _targetYPosition;
    [SerializeField] private float _scaleDuration;
    [SerializeField] private float _scaleMultiplier;
    [SerializeField] private Activator _activator;
    [SerializeField] private AudioClip _clip;
    [SerializeField] private Canvas _canvas;

    private Vector3 _targetScale;
    private Sequence _currentSequence;
    private Voiceover _voiceover;
    private ZoomChanger _zoomChanger;

    private void Awake()
    {
        _voiceover = GetComponent<Voiceover>();
        _zoomChanger = new ZoomChanger();

        if (_zoomChanger.IsMobileWithTallScreen() == false)
        {
            _scaleMultiplier = 0.5f;
            _targetYPosition *= 2;
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

    public void ZoomIn(float time)
    {
        StopCurrentAnimation();

        _voiceover.Play(_clip);

        _currentSequence = DOTween.Sequence();

        GetCreatedSequence(time).OnComplete(() =>
        {
            _activator.gameObject.SetActive(false);
            _currentSequence = null;
        });
    }

    public void Demonstrate()
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