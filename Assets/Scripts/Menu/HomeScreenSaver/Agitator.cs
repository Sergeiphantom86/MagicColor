using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Viewer), typeof(PixelSpawner))]
public class Agitator : MonoBehaviour, IAnimatable
{
    private const float MinDirectionValue = -1f;
    private const float MaxDirectionValue = 1f;
    private const float AxisValueZ = 0f;

    private float _delay;
    private float _explosionForce;
    private float _explosionDuration;
    private float _rotationIntensity;
    private float _scaleDownDuration;
    private float _delayBeforeDestroy;

    private Viewer _viewer;
    private PixelSpawner _pixelSpawner;
    private Sequence _explosionSequence;

    private void Awake()
    {
        _delay = 1f;
        _explosionForce = 50f;
        _explosionDuration = 1f;
        _rotationIntensity = 360f;
        _scaleDownDuration = 0.5f;
        _delayBeforeDestroy = 0.1f;

        _viewer = GetComponent<Viewer>();
        _pixelSpawner = GetComponent<PixelSpawner>();
    }

    public void PauseAnimations()
    {
        _explosionSequence?.Pause();
    }

    public void ResumeAnimations()
    {
        if (_explosionSequence != null && !_explosionSequence.IsPlaying() && _explosionSequence.IsActive())
        {
            _explosionSequence.Play();
        }
    }

    public void TriggerExplosion(List<Fragment> pixels)
    {
        _explosionSequence?.Kill();
        _explosionSequence = DOTween.Sequence();

        _explosionSequence.AppendInterval(0.1f);

        foreach (Fragment pixel in pixels)
        {
            if (pixel == null || !pixel.gameObject.activeInHierarchy) continue;

            AddPixelToExplosionSequence(pixel);
        }

        _explosionSequence.OnComplete(() =>
        {
            _pixelSpawner.Clear();

            DOVirtual.DelayedCall(_delay, () => _viewer.ShowNextSprite());
        });

        _explosionSequence.Play();
    }

    private void AddPixelToExplosionSequence(Fragment pixel)
    {
        if (pixel == null) return;

        Sequence pixelSequence = CreatePixelExplosionSequence(pixel);
        _explosionSequence.Join(pixelSequence);
    }

    private Sequence CreatePixelExplosionSequence(Fragment pixel)
    {
        Sequence sequence = DOTween.Sequence();

        AddMovementAnimation(sequence, pixel);
        AddRotationAnimation(sequence, pixel);
        AddScalingAnimation(sequence, pixel);

        sequence.AppendInterval(_delayBeforeDestroy);

        sequence.OnComplete(() => DeactivatePixel(pixel));

        return sequence;
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
            Random.Range(MinDirectionValue, MaxDirectionValue),
            Random.Range(MinDirectionValue, MaxDirectionValue),
            AxisValueZ).normalized;
    }

    private Vector3 GetRandomTargetRotation(float rotationZ)
    {
        return new Vector3(0, 0, rotationZ + Random.Range(-_rotationIntensity, _rotationIntensity));
    }

    private void OnDestroy()
    {
        _explosionSequence?.Kill();
    }
}