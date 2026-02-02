using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Voiceover))]
public class BounceRotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private WheelAnimator _wheelAnimator;
    [SerializeField] private AudioClip _audioClip;

    private bool _isRotating;
    private bool _useCooldown;
    private int _rotationAngle;
    private float _cooldownTime;
    private float _lastRotationTime;
    private float _rotationDuration;
    private Vector3 _originalRotation;
    private Sequence _rotationSequence;
    private Voiceover _voiceover;

    private void Awake()
    {
        _useCooldown = false;
        _cooldownTime = 1f;
        _rotationAngle = 60;
        _rotationDuration = 0.05f;
        _voiceover = GetComponent<Voiceover>();

        _originalRotation = transform.localEulerAngles;
    }

    private void OnEnable()
    {
        _wheelAnimator.OnThresholdPassed += PlayRotation;
    }

    private void OnDisable()
    {
        _wheelAnimator.OnThresholdPassed -= PlayRotation;

        ResetRotation();
    }

    public void PlayRotation()
    {
        if (_isRotating) return;
        if (_useCooldown && Time.time - _lastRotationTime < _cooldownTime) return;
        _voiceover.Play(_audioClip);
        _isRotating = true;
        _lastRotationTime = Time.time;
        
        ResetRotation();

        _rotationSequence = DOTween.Sequence();

        _rotationSequence.Append(transform.DOLocalRotate(GetTurn(), _rotationDuration));

        _rotationSequence.OnComplete(() =>
        {
            _isRotating = false;
            ResetToOriginalRotation();
        });
    }

    private Vector3 GetTurn()
    {
        return new Vector3(_originalRotation.x, _originalRotation.y, _originalRotation.z - _rotationAngle);
    }

    private void ResetRotation()
    {
        _rotationSequence?.Kill();
        _rotationSequence = null;
        transform.DOKill();
    }

    private void ResetToOriginalRotation()
    {
        transform.localEulerAngles = _originalRotation;
    }

    private void OnDestroy()
    {
        ResetRotation();
    }
}