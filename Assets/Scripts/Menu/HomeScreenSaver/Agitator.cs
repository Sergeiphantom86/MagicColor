using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Viewer), typeof(PixelSpawner))]
public class Agitator : MonoBehaviour, IAnimatable
{
    [SerializeField] private ParticleSystem _particleSystem;

    private const float MinDirectionValue = -1f;
    private const float MaxDirectionValue = 1f;
    private const float AxisValueZ = 0f;

    private float _delay;
    private float _explosionForce;
    private float _explosionDuration;
    private float _rotationIntensity;
    private float _scaleDownDuration;
    private float _delayBeforeDestroy;
    private float _interval;
    private Viewer _viewer;
    private PixelSpawner _pixelSpawner;
    private Sequence _explosionSequence;
    private Sequence _sequence;

    private void Awake()
    {
        _delay = 1f;
        _interval = 0.1f;
        _explosionForce = 50f;
        _explosionDuration = 1f;
        _rotationIntensity = 360f;
        _scaleDownDuration = 0.5f;
        _delayBeforeDestroy = 0.1f;

        _viewer = GetComponent<Viewer>();
        _pixelSpawner = GetComponent<PixelSpawner>();
    }

    public void PauseAnimations() =>
        DOTweenExtensions.SafePause(_explosionSequence);

    public void ResumeAnimations() =>
        DOTweenExtensions.SafePlay(_explosionSequence);

    public void TriggerExplosion(List<Fragment> pixels)
    {
        this.SafeDelayedCall(_delay, () => SafeWaitExplosion(pixels));
    }

    private void SafeWaitExplosion(List<Fragment> pixels)
    {
        if (isActiveAndEnabled == false) return;

        DOTweenExtensions.SafeKill(_explosionSequence);

        _explosionSequence = DOTween.Sequence();

        _explosionSequence.AppendInterval(_interval);

        AddPixelToExplosionSequence(pixels);

        CompleteSequence();

        _explosionSequence.Play();

        TurnOffParticleSystem();
    }

    private void AddPixelToExplosionSequence(List<Fragment> pixels)
    {
        foreach (Fragment pixel in pixels)
        {
            if (pixel == null || pixel.gameObject.activeInHierarchy == false) continue;

            _explosionSequence.Join(CreatePixelExplosionSequence(pixel));
        }
    }

    private void CompleteSequence()
    {
        _explosionSequence.OnComplete(() =>
        {
            _pixelSpawner.Clear();

            this.SafeDelayedCall(_delay, () => _viewer.ShowNextSprite());
        });
    }

    private void TurnOffParticleSystem()
    {
        if (_particleSystem != null)
        {
            _particleSystem.Stop();
            _particleSystem.gameObject.SetActive(false);
        }
    }

    private Sequence CreatePixelExplosionSequence(Fragment pixel)
    {
        _sequence = DOTween.Sequence();

        AddMovementAnimation(_sequence, pixel);
        AddRotationAnimation(_sequence, pixel);
        AddScalingAnimation(_sequence, pixel);

        _sequence.AppendInterval(_delayBeforeDestroy);

        _sequence.OnComplete(() => DeactivatePixel(pixel));

        return _sequence;
    }

    private void AddMovementAnimation(Sequence sequence, Fragment pixel)
    {
        Vector3 targetPosition = GetTargetPosition(pixel.transform.position);

        sequence.Append(pixel.transform
            .DOMove(targetPosition, _explosionDuration)
            .SetEase(Ease.OutQuad));
    }

    private void AddRotationAnimation(Sequence sequence, Fragment pixel)
    {
        Vector3 targetRotation = GetRandomTargetRotation(pixel.transform.rotation.eulerAngles.z);
        sequence.Join(pixel.transform
            .DORotate(targetRotation, _explosionDuration)
            .SetEase(Ease.OutQuad));
    }

    private void AddScalingAnimation(Sequence sequence, Fragment pixel)
    {
        sequence.Append(pixel.transform
            .DOScale(Vector3.zero, _scaleDownDuration)
            .SetEase(Ease.InBack));
    }

    private void DeactivatePixel(Fragment pixel)
    {
        if (pixel != null && pixel.gameObject != null)
        {
            pixel.gameObject.SetActive(false);
        }
    }

    private Vector3 GetTargetPosition(Vector3 originalPosition)
    {
        return originalPosition + GetRandomExplosionDirection() * _explosionForce;
    }

    private Vector3 GetRandomExplosionDirection()
    {
        return new Vector3(
            GetDirectionValue(),
            GetDirectionValue(),
            AxisValueZ).normalized;
    }

    private float GetDirectionValue()
    {
        return Random.Range(MinDirectionValue, MaxDirectionValue);
    }

    private Vector3 GetRandomTargetRotation(float rotationZ)
    {
        return new Vector3(0, 0, rotationZ + Random.Range(-_rotationIntensity, _rotationIntensity));
    }

    private void OnDestroy()
    {
        DOTweenExtensions.SafeKill(_explosionSequence);
    }
}