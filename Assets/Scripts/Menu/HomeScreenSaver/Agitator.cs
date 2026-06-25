using System.Collections.Generic;
using DG.Tweening;
using PuzzleEditor;
using PuzzleEditor.PoolEffects;
using PuzzleEditor.SoundEditor;
using UnityEngine;
namespace Menu.HomeScreenSaver
{

[RequireComponent(typeof(Viewer), typeof(TextureInitializer))]
public class Agitator : MonoBehaviour, IAnimatable
{
    private const float MinDirectionValue = -1f;
    private const float MaxDirectionValue = 1f;
    private const float AxisValueZ = 0f;

    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private Effecter _destruction;
    [SerializeField] private AudioClip _destructionSound;

    private float _interval;
    private float _explosionForce;
    private float _explosionDuration;
    private float _rotationIntensity;
    private float _scaleDownDuration;
    private float _delayBeforeDestroy;
    private Sequence _sequence;
    private Voiceover _voiceover;
    private Sequence _explosionSequence;
    private PixelShine _pixelShine;

    public event System.Action Exploded;

    private void Awake()
    {
        _interval = 1;
        _explosionForce = 50f;
        _explosionDuration = 1f;
        _rotationIntensity = 360f;
        _scaleDownDuration = 0.5f;
        _delayBeforeDestroy = 0.1f;

        _voiceover = GetComponent<Voiceover>();
        _pixelShine = GetComponent<PixelShine>();
    }

    private void OnEnable()
    {
        _pixelShine.Glistened += TriggerExplosion;
    }

    private void OnDisable()
    {
        _pixelShine.Glistened -= TriggerExplosion;
    }

    public void PauseAnimations() =>
        DOTweenExtensions.SafePause(_explosionSequence);

    public void ResumeAnimations() =>
        DOTweenExtensions.SafePlay(_explosionSequence);

    public void TriggerExplosion(List<Fragment> pixels)
    {
        if (pixels == null || pixels.Count == 0) 
            return;

        SafeWaitExplosion(pixels);
    }

    private void SafeWaitExplosion(List<Fragment> pixels)
    {
        if (isActiveAndEnabled == false) 
            return;

        DOTweenExtensions.SafeKill(_explosionSequence);

        _explosionSequence = DOTween.Sequence();

        _explosionSequence.AppendInterval(_interval);

        AddPixelToExplosionSequence(pixels);

        CompleteSequence();

        PlayEffects();

        TurnOffParticleSystem();
    }

    private void PlayEffects()
    {
        if (_voiceover != null && _destructionSound != null)
        {
            _voiceover.PlayOneShot(_destructionSound);
        }

        if (_destruction != null)
        {
            _destruction.CraeteParticles(_particleSystem.transform.position, Quaternion.identity, transform.localScale.x);

            _explosionSequence.Play();
        }
    }

    private void CompleteSequence()
    {
        _explosionSequence.Play()
            .OnComplete(() =>
        {
            Exploded?.Invoke();
        });
    }

    private void AddPixelToExplosionSequence(List<Fragment> pixels)
    {
        foreach (Fragment pixel in pixels)
        {
            if (pixel == null || pixel.gameObject.activeInHierarchy == false) 
                continue;

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
}