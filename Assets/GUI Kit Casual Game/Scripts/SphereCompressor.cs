using UnityEngine;
using DG.Tweening;
using System.Collections;

public class SphereCompressor : MonoBehaviour
{
    [Header("Compression Settings")]
    [SerializeField] private float _compressionStrength = 0.5f;
    [SerializeField] private float _compressionDuration = 0.5f;
    [SerializeField] private float _recoveryDuration = 0.3f;
    [SerializeField] private Ease _compressionEase = Ease.OutBack;
    [SerializeField] private Ease _recoveryEase = Ease.OutElastic;

    private Vector3 _originalScale;
    private Sequence _compressionSequence;

    private void Awake()
    {
        _originalScale = transform.localScale;
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1.0f);

        
    }

    private void OnCollisionEnter(Collision collision)
    {
        CompressFromImpact(collision.transform.position, 1, 3);
    }

    [ContextMenu("Compress Sphere")]
    public void CompressSphere()
    {
        StopCompression();

        _compressionSequence = DOTween.Sequence();

        // Сжатие по оси Y, растяжение по X и Z
        Vector3 compressedScale = new Vector3(
            _originalScale.x * (1f + _compressionStrength),
            _originalScale.y * (1f - _compressionStrength),
            _originalScale.z * (1f + _compressionStrength)
        );

        _compressionSequence.Append(transform.DOScale(compressedScale, _compressionDuration)
            .SetEase(_compressionEase));

        _compressionSequence.Append(transform.DOScale(_originalScale, _recoveryDuration)
            .SetEase(_recoveryEase));
    }

    public void CompressInDirection(Vector3 direction, float strength)
    {
        StopCompression();

        // Нормализуем направление и создаем масштаб для сжатия
        direction = direction.normalized;

        Vector3 compressedScale = _originalScale;

        // Сжимаем в направлении (уменьшаем масштаб по направлению)
        compressedScale.x *= (1f - Mathf.Abs(direction.x) * strength);
        compressedScale.y *= (1f - Mathf.Abs(direction.y) * strength);
        compressedScale.z *= (1f - Mathf.Abs(direction.z) * strength);

        // Растягиваем в перпендикулярных направлениях
        compressedScale.x *= (1f + (1f - Mathf.Abs(direction.x)) * strength * 0.5f);
        compressedScale.y *= (1f + (1f - Mathf.Abs(direction.y)) * strength * 0.5f);
        compressedScale.z *= (1f + (1f - Mathf.Abs(direction.z)) * strength * 0.5f);

        _compressionSequence = DOTween.Sequence();
        _compressionSequence.Append(transform.DOScale(compressedScale, _compressionDuration));
        _compressionSequence.Append(transform.DOScale(_originalScale, _recoveryDuration));
    }

    public void CompressFromImpact(Vector3 impactPoint, float strength, float radius = 2f)
    {
        StopCompression();

        // Вычисляем направление от точки удара к центру сферы
        Vector3 directionToCenter = (transform.position - impactPoint).normalized;

        // Вычисляем силу сжатия на основе расстояния
        float distance = Vector3.Distance(transform.position, impactPoint);
        float distanceFactor = Mathf.Clamp01(1f - (distance / radius));
        float actualStrength = strength * distanceFactor;

        if (actualStrength > 0.01f)
        {
            CompressInDirection(directionToCenter, actualStrength);
        }
    }

    private void StopCompression()
    {
        _compressionSequence?.Kill();
        transform.localScale = _originalScale;
    }

    private void OnDestroy()
    {
        StopCompression();
    }
}