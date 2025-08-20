using UnityEngine;
using UnityEngine.UI;

public class ParticleSystemController : MonoBehaviour
{
    [SerializeField] private GameObject _spritePrefab;
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private CoinWallet _coinWallet;
    [SerializeField] private CrystalWallet _crystalWallet;
    [SerializeField] private float _explosionRadius;
    [SerializeField] private float _minScale;
    [SerializeField] private float _maxScale;
    [SerializeField] private float _firstPhaseRatio;

    private float _duration;
    private Vector2 _directionRange;
    private float _directionValueY;
    private int _quantityMultiplier;
    private float _moveToTargetDuration;
    private float _explosionDistanceMultiplier;

    private void Awake()
    {
        _duration = 0.2f;
        _directionValueY = 1f;
        _quantityMultiplier = 5;
        _moveToTargetDuration = 0.5f;
        _explosionDistanceMultiplier = 20f;
        _directionRange = new Vector2(-2f, 2f);
    }

    public void ActivateAtPosition(Award item)
    {
        int particleCount = GetNumberParticles(item);
    
        for (int i = 0; i < particleCount; i++)
        {
            CreateParticle(item);
        }

        HandleParticleComplete(item, GetDuration());
    }

    private float GetDuration()
    {
        return _duration + _moveToTargetDuration;
    }

    private int GetNumberParticles(Award item)
    {
        return item is Crystal
            ? item.Value / _quantityMultiplier
            : item.Value * _quantityMultiplier;
    }

    private void CreateParticle(Award item)
    {
        GameObject particle = Instantiate(_spritePrefab, transform.position, Quaternion.Euler(0, 0, Random.Range(0, 360f)), transform);

        particle.transform.localScale = Vector3.zero;

        if (particle.TryGetComponent(out Image image))
        {
            image.sprite = item.Sprite;
        }

        ParticleAnimation anim = particle.AddComponent<ParticleAnimation>();

        anim.Initialize(
            randomPosition: CalculateRandomPosition(),
            targetPosition: _targetTransform.position,
            settings: GetParticleAnimation());
    }

    private ParticleAnimation.Settings GetParticleAnimation()
    {
        return new ParticleAnimation.Settings(
                minScale: _minScale,
                maxScale: _maxScale,
                scaleUpDuration: _duration,
                moveToRandomDuration: _duration,
                moveToTargetDuration: _moveToTargetDuration,
                firstPhaseRatio: _firstPhaseRatio);
    }

    private void HandleParticleComplete(Award item, float duration)
    {
        if (item is Crystal)
        {
            _crystalWallet.AddFunds(item.Value, duration);
            return;
        }

        _coinWallet.AddFunds(item.Value * item.Winn, duration);
    }

    private Vector3 CalculateRandomPosition()
    {
        return transform.position + GetRandomDirection() * GetDistance();
    }

    private float GetDistance() =>
        Random.Range(_explosionRadius, _explosionRadius * _explosionDistanceMultiplier);

    private Vector3 GetRandomDirection()
    {
        return new Vector3(Random.Range(_directionRange.x, _directionRange.y), _directionValueY, 0).normalized;
    }
}