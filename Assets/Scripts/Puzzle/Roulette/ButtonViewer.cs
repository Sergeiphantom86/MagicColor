using DG.Tweening;
using UnityEngine;

public class ButtonViewer : MonoBehaviour
{
    [SerializeField] private Exit _exit;

    private float _duration;
    private Vector3 _endScale;
    private Vector3 _startScale;
    private Vector3 _targetPosition;
    private Sequence _sequence;
    private Transform _transform;

    private void Awake()
    {
        _duration = 0.3f;
        _transform = transform;

        //SetEndScale();
        //SetStartScale();
    }

    private void OnEnable()
    {
        _exit.OnShow += Show;
    }

    private void OnDisable()
    {
        _exit.OnShow -= Show;
    }

    private void OnDestroy()
    {
        if (_sequence != null && _sequence.IsActive())
        {
            _sequence.Kill();
        }
    }

    public void Show(Vector3 startPosition)
    {
        CacheTargetPosition();
        MoveToStartPosition(startPosition);
        ResetSequence();
        PlayShowAnimation();
    }

    private void CacheTargetPosition()
    {
        _targetPosition = _transform.position;
    }

    private void MoveToStartPosition(Vector3 position)
    {
        _transform.position = position;
    }

    private void ResetSequence()
    {
        if (_sequence == null || !_sequence.IsActive())
            return;

        _sequence.Kill();
        _sequence = null;
    }

    private void PlayShowAnimation()
    {
        _sequence = DOTween.Sequence();

        Move();

        Scale();
    }

    private void Move()
    {
        _sequence.Join(
           _transform.DOMove(_targetPosition, _duration)
               .SetEase(Ease.Linear)
       );
    }

    private void Scale()
    {
        _sequence.Join(
           _transform.DOScale(_endScale, _duration)
               .SetEase(Ease.OutBack)
       );
    }


    private void SetStartScale()
    {
        _startScale = Vector3.zero;
        _transform.localScale = _startScale;
    }

    private void SetEndScale()
    {
        _endScale = _transform.localScale;
    }
}