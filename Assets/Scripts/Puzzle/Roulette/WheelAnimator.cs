using DG.Tweening;
using System;
using UnityEngine;

public class WheelAnimator : MonoBehaviour
{
    [SerializeField] private float _pointerAngle;

    private int _spinDuration;
    private int _minFullRotations;
    private bool _isSpinning;
    private float _accumulatedRotation;
    private Sequence _spinSequence;
    private Transform _transform;
    private ParticleSystem _particleSystem;
    private Vector3 _position;
    private float _startRotation;
    private float _nextThreshold;
    private bool _firstThresholdPassed;

    public event Action OnThresholdPassed; 

    private void Awake()
    {
        _particleSystem = GetComponentInChildren<ParticleSystem>(true);

        if (_particleSystem == null)
        {
            Debug.LogError("ParticleSystem не найден!!!");
            return;
        }

        _particleSystem.gameObject.SetActive(false);
        _position = _particleSystem.transform.position;
        _spinDuration = 3;
        _minFullRotations = 3;
        _transform = transform;
    }

    void Start()
    {
        DOTween.Init();
        _accumulatedRotation = transform.eulerAngles.z;
    }

    public void SpinToTarget(float sectorAngle, Action onComplete)
    {
        if (_isSpinning) return;

        _isSpinning = true;
        _spinSequence?.Kill();

        _startRotation = _accumulatedRotation;
        _firstThresholdPassed = false;
        _nextThreshold = 22.5f;

        SetupSpinSequence(sectorAngle, onComplete);
        _spinSequence.Play();
    }

    private void SetupSpinSequence(float sectorAngle, Action onComplete)
    {
        _spinSequence = DOTween.Sequence();

        _particleSystem.gameObject.SetActive(false);

        Tween rotationTween = CreateRotationTween(sectorAngle);

        rotationTween.OnUpdate(CheckRotationProgress);

        _spinSequence.Append(rotationTween);

        _spinSequence.OnComplete(() =>
        {
            CompleteSpin(sectorAngle);
            onComplete?.Invoke();

            _particleSystem.transform.position = _position;
            _particleSystem.gameObject.SetActive(true);
        });
    }

    private void CheckRotationProgress()
    {
        if (Mathf.Abs(_accumulatedRotation - _startRotation) >= _nextThreshold)
        {
            OnThresholdPassed?.Invoke();

            if (_firstThresholdPassed == false)
            {
                _firstThresholdPassed = true;
                _nextThreshold = 67.5f;
                return;
            }

            _nextThreshold += 45f;
        }
    }

    private Tween CreateRotationTween(float sectorAngle)
    {
        return DOTween.To(
            getter: GetCurrentRotation,
            setter: UpdateRotation,
            endValue: GetTotalRotation(CalculateRequiredRotation(sectorAngle)),
            duration: _spinDuration
        ).SetEase(Ease.OutCubic);
    }

    private void UpdateRotation(float currentRotation)
    {
        _accumulatedRotation = currentRotation;
        transform.eulerAngles = new Vector3(_transform.eulerAngles.x, _transform.eulerAngles.y, currentRotation);
    }

    private float GetCurrentRotation() => _accumulatedRotation;

    private void CompleteSpin(float sectorAngle)
    {
        float requiredRotation = CalculateRequiredRotation(sectorAngle);
        _accumulatedRotation = GetAccumulatedSpin(requiredRotation);
        _isSpinning = false;
    }

    private float GetAccumulatedSpin(float requiredRotation)
    {
        return _accumulatedRotation + requiredRotation;
    }

    private float GetTotalRotation(float requiredRotation)
    {
        return _accumulatedRotation + requiredRotation + 360f * _minFullRotations;
    }

    private float CalculateRequiredRotation(float sectorAngle)
    {
        float angleDifference = _pointerAngle - (_accumulatedRotation + sectorAngle) % 360f;

        if (angleDifference > 180f) angleDifference -= 360f;
        if (angleDifference < -180f) angleDifference += 360f;

        return angleDifference;
    }

    public bool IsSpinning => _isSpinning;

    void OnDestroy()
    {
        _spinSequence?.Kill();
    }
}