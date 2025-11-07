using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIAnimator : MonoBehaviour, IUIAnimator
{
    [SerializeField] private float _positionX;
    [SerializeField] private float _positionY;

    private float _scale;
    private float _duration;
    private MoverUI _moverUI;
    private Sequence _sequence;
    private RectTransform _rectTransform;

    private Vector3 _startPosition;

    private void Awake()
    {
        _scale = 2;
        _duration = 1;
        _moverUI = new MoverUI();
        _sequence = DOTween.Sequence();
        _startPosition = transform.position;
        _rectTransform = GetComponent<RectTransform>();
    }

    public Sequence Move(RectTransform canvasRect)
    {
        return GetSequence(canvasRect, _positionX, _positionY, 0);
    }

    public void Return(RectTransform canvasRect)
    {
        GetSequence(canvasRect, _startPosition.x, _startPosition.y, 0);
    }

    public void Increase()
    {
        CreateOrRestartSequence();

        _moverUI.EnableAnimationResizing(_rectTransform, _duration, _scale, _scale);
    }

    private Sequence GetSequence(RectTransform canvasRect, float positionX, float positionY, float positionZ)
    {
        CreateOrRestartSequence();

        return _moverUI.EnableMotionAnimation(_rectTransform, _duration, canvasRect, positionX, positionY, positionZ);
    }

    private void CreateOrRestartSequence()
    {
        if (_sequence != null && _sequence.IsActive())
        {
            _sequence.Kill();
        }

        _sequence = DOTween.Sequence();
    }

    private void OnDestroy()
    {
        if (_sequence != null && _sequence.IsActive())
        {
            _sequence.Kill();
        }
    }
}