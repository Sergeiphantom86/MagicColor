using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Viewer), typeof(TextureInitializer))]
public class Agitator : MonoBehaviour, IAnimatable
{
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private ParticleSystemPool _destruction;

    private const float MinDirectionValue = -1f;
    private const float MaxDirectionValue = 1f;
    private const float AxisValueZ = 0f;

    private float _delay;
    private float _interval;
    private float _explosionForce;
    private float _explosionDuration;
    private float _rotationIntensity;
    private float _scaleDownDuration;
    private float _delayBeforeDestroy;
    private Viewer _viewer;
    private Sequence _sequence;
    private Sequence _explosionSequence;
    private TextureInitializer _textureInitializer;

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
        _textureInitializer = GetComponent<TextureInitializer>();
    }

    public void PauseAnimations() =>
        DOTweenExtensions.SafePause(_explosionSequence);

    public void ResumeAnimations() =>
        DOTweenExtensions.SafePlay(_explosionSequence);

    public void TriggerExplosion(List<Fragment> pixels)
    {
        if (pixels == null || pixels.Count == 0) return;

        this.SafeDelayedCall(_delay, () => SafeWaitExplosion(pixels));
    }

    private void SafeWaitExplosion(List<Fragment> pixels)
    {
        if (isActiveAndEnabled == false) return;

        DOTweenExtensions.SafeKill(_explosionSequence);

        _explosionSequence = DOTween.Sequence();

        _explosionSequence.AppendInterval(_interval);

        AddPixelToExplosionSequence(pixels);

        CompleteSequence(pixels);

        if (_destruction != null)
        {
            ParticleSystem particleSystem = _destruction.Pool.Get();
            _destruction.CreateEffect(GetEffect(particleSystem));
            _destruction.Return(particleSystem);
            _explosionSequence.Play();
        }

        TurnOffParticleSystem();
    }

    private void CompleteSequence(List<Fragment> pixels)
    {
        _explosionSequence.OnComplete(() =>
        {
 
            _textureInitializer.ClearAllFragments();

            this.SafeDelayedCall(_delay, () => {
                if (_viewer != null && isActiveAndEnabled)
                {
                    _viewer.ShowNextSprite();
                }
            });
        });

        _explosionSequence.Play();
    }

    private void AddPixelToExplosionSequence(List<Fragment> pixels)
    {
        foreach (Fragment pixel in pixels)
        {
            if (pixel == null || pixel.gameObject.activeInHierarchy == false) continue;

            _explosionSequence.Join(CreatePixelExplosionSequence(pixel));
        }
    }

    private Sequence CreatePixelExplosionSequence(Fragment pixel)
    {
        _sequence = DOTween.Sequence();

        AddMovementAnimation(_sequence, pixel);
        AddRotationAnimation(_sequence, pixel);
        AddScalingAnimation(_sequence, pixel);

        _sequence.AppendInterval(_delayBeforeDestroy);

        _sequence.OnComplete(() => ResetFragmentState(pixel));

        return _sequence;
    }

    private void ResetFragmentState(Fragment pixel)
    {
        if (pixel != null)
        {
            pixel.transform.localScale = Vector3.one;
            pixel.transform.rotation = Quaternion.identity;
        }
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

    private ParticleSystem GetEffect(ParticleSystem particleSystem)
    {
        particleSystem.transform.position = _particleSystem.transform.position;
        particleSystem.transform.localScale = gameObject.transform.localScale;
        particleSystem.gameObject.SetActive(true);

        return particleSystem;
    }

    private void TurnOffParticleSystem()
    {
        if (_particleSystem != null)
        {
            _particleSystem.Stop();
            _particleSystem.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        DOTweenExtensions.SafeKill(_explosionSequence);
    }
}