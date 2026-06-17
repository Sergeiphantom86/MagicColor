using DG.Tweening;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    private Rotator _rotation;
    private IProgressSaver _progressSaver;
    private Tween _moveTween;
    private float _positionX;
    private float _durationDivider;

    private void Awake()
    {
        _positionX = 50;
        _durationDivider = 10;
        _rotation = GetComponent<Rotator>();
        _progressSaver = new ProgressSaver();
    }

    private void Start()
    {
        _progressSaver.SaveProgress();
    }

    public void Return(float duration)
    {
        MoveX(duration);
    }

    public void StartRotation()
    {
        _rotation.StartRotation();
    }

    private void MoveX(float duration)
    {
        _moveTween?.Kill();
        _moveTween = transform.DOMoveX(GetPositionX(), GetDuration(duration))
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            gameObject.SetActive(false));
    }

    private float GetPositionX()
    {
        return transform.position.x - _positionX;
    }

    private float GetDuration(float duration)
    {
        return duration / _durationDivider;
    }

    private void OnDestroy()
    {
        _moveTween?.Kill();
    }
}