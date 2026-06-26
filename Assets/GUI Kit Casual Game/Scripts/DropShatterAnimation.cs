using UnityEngine;
using DG.Tweening;
using System.Collections;

public class DropShatterAnimation : MonoBehaviour
{
    [Header("Main Settings")]
    [SerializeField] private float _shatterDuration = 0.8f;
    [SerializeField] private float _reverseDuration = 1f;
    [SerializeField] private float _dropHeight = 3f;
    [SerializeField] private float _shatterForce = 2f;

    [Header("Drop Parts")]
    [SerializeField] private Transform _mainDrop;
    [SerializeField] private Transform[] _shatterPieces;
    [SerializeField] private ParticleSystem _splashParticles;

    [Header("Ease Settings")]
    [SerializeField] private Ease _fallEase = Ease.InCubic;
    [SerializeField] private Ease _shatterEase = Ease.OutQuad;
    [SerializeField] private Ease _reverseEase = Ease.OutBack;

    private Vector3[] _originalPositions;
    private Vector3[] _originalScales;
    private Quaternion[] _originalRotations;
    private Sequence _shatterSequence;
    private Sequence _reverseSequence;

    private bool _isAnimating;
    private Vector3 _originalDropPosition;

    private void Awake()
    {
        ValidateComponents();
        StoreOriginalTransforms();
        _originalDropPosition = _mainDrop.position;

        // ���������� �������� �������
        SetShatterPiecesActive(false);

        PlayShatterAnimation();

        PlayReverseAnimation();
    }

    private void ValidateComponents()
    {
        if (_mainDrop == null)
            Debug.LogError("Main Drop is not assigned!", this);

        if (_shatterPieces == null || _shatterPieces.Length == 0)
            Debug.LogError("Shatter Pieces are not assigned!", this);
    }

    private void StoreOriginalTransforms()
    {
        _originalPositions = new Vector3[_shatterPieces.Length];
        _originalScales = new Vector3[_shatterPieces.Length];
        _originalRotations = new Quaternion[_shatterPieces.Length];

        for (int i = 0; i < _shatterPieces.Length; i++)
        {
            if (_shatterPieces[i] != null)
            {
                _originalPositions[i] = _shatterPieces[i].localPosition;
                _originalScales[i] = _shatterPieces[i].localScale;
                _originalRotations[i] = _shatterPieces[i].localRotation;
            }
        }
    }

    [ContextMenu("Play Shatter Animation")]
    public void PlayShatterAnimation()
    {
        if (_isAnimating) return;

        StopAllAnimations();
        StartCoroutine(ShatterAnimationRoutine());
    }

    [ContextMenu("Play Reverse Animation")]
    public void PlayReverseAnimation()
    {
        if (_isAnimating) return;

        StopAllAnimations();
        StartCoroutine(ReverseAnimationRoutine());
    }

    private IEnumerator ShatterAnimationRoutine()
    {
        _isAnimating = true;

        // 1. ������� �����
        yield return FallAnimation();

        // 2. �������� � ����� ��������
        yield return ShatterEffect();

        _isAnimating = false;
    }

    private IEnumerator ReverseAnimationRoutine()
    {
        _isAnimating = true;

        // 1. ������ �������� �������
        yield return ReverseShatterEffect();

        // 2. ������ �����
        yield return RiseAnimation();

        _isAnimating = false;
    }

    private IEnumerator FallAnimation()
    {
        _mainDrop.gameObject.SetActive(true);

        // ���������� ������� �����
        _mainDrop.position = _originalDropPosition + Vector3.up * _dropHeight;
        _mainDrop.localScale = Vector3.one;

        // �������� �������
        var fallTween = _mainDrop.DOMoveY(_originalDropPosition.y, _shatterDuration * 0.6f)
            .SetEase(_fallEase);

        // ������ ����������� ��� �����
        var squashTween = _mainDrop.DOScale(new Vector3(1.2f, 0.8f, 1.2f), _shatterDuration * 0.2f)
            .SetDelay(_shatterDuration * 0.5f)
            .SetEase(Ease.OutBounce);

        yield return fallTween.WaitForCompletion();
    }

    private IEnumerator ShatterEffect()
    {
        // ������������� ������� �����
        if (_splashParticles != null)
            _splashParticles.Play();

        // �������� �������� �����
        _mainDrop.DOScale(Vector3.zero, _shatterDuration * 0.3f)
            .SetEase(Ease.InBack);

        // ���������� � ������������ �������
        SetShatterPiecesActive(true);
        ResetPiecesToOrigin();

        _shatterSequence = DOTween.Sequence();

        for (int i = 0; i < _shatterPieces.Length; i++)
        {
            if (_shatterPieces[i] == null) continue;

            // ��������� ����������� ������
            Vector3 randomDirection = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(0.2f, 1f),
                Random.Range(-1f, 1f)
            ).normalized;

            Vector3 targetPosition = _originalPositions[i] + randomDirection * _shatterForce;
            Vector3 targetRotation = new Vector3(
                Random.Range(0, 360f),
                Random.Range(0, 360f),
                Random.Range(0, 360f)
            );

            _shatterSequence.Join(_shatterPieces[i].DOLocalMove(targetPosition, _shatterDuration)
                .SetEase(_shatterEase));

            _shatterSequence.Join(_shatterPieces[i].DOLocalRotate(targetRotation, _shatterDuration)
                .SetEase(_shatterEase));
        }

        yield return _shatterSequence.WaitForCompletion();
    }

    private IEnumerator ReverseShatterEffect()
    {
        _reverseSequence = DOTween.Sequence();

        for (int i = 0; i < _shatterPieces.Length; i++)
        {
            if (_shatterPieces[i] == null) continue;

            _reverseSequence.Join(_shatterPieces[i].DOLocalMove(_originalPositions[i], _reverseDuration)
                .SetEase(_reverseEase));

            _reverseSequence.Join(_shatterPieces[i].DOLocalRotateQuaternion(_originalRotations[i], _reverseDuration)
                .SetEase(_reverseEase));
        }

        yield return _reverseSequence.WaitForCompletion();

        // �������� ������� ����� ������
        SetShatterPiecesActive(false);
    }

    private IEnumerator RiseAnimation()
    {
        // ���������� � ��������� ������ �����
        _mainDrop.gameObject.SetActive(true);
        _mainDrop.position = _originalDropPosition;
        _mainDrop.localScale = Vector3.zero;

        var scaleTween = _mainDrop.DOScale(Vector3.one, _reverseDuration * 0.4f)
            .SetEase(Ease.OutBack);

        var riseTween = _mainDrop.DOMoveY(_originalDropPosition.y + _dropHeight, _reverseDuration * 0.6f)
            .SetDelay(_reverseDuration * 0.2f)
            .SetEase(Ease.OutCubic);

        yield return riseTween.WaitForCompletion();
    }

    private void SetShatterPiecesActive(bool isActive)
    {
        foreach (var piece in _shatterPieces)
        {
            if (piece != null)
                piece.gameObject.SetActive(isActive);
        }
    }

    private void ResetPiecesToOrigin()
    {
        for (int i = 0; i < _shatterPieces.Length; i++)
        {
            if (_shatterPieces[i] != null)
            {
                _shatterPieces[i].localPosition = _originalPositions[i];
                _shatterPieces[i].localScale = _originalScales[i];
                _shatterPieces[i].localRotation = _originalRotations[i];
            }
        }
    }

    private void StopAllAnimations()
    {
        _shatterSequence?.Kill();
        _reverseSequence?.Kill();
        StopAllCoroutines();
        _isAnimating = false;
    }

    private void OnDestroy()
    {
        StopAllAnimations();
    }

    // �������������� ��������� ����� ����������� ����
    [ContextMenu("Setup Drop Pieces")]
    private void SetupDropPieces()
    {
        // ���� ����� ������� ������ ��������� ������� � ���������
        var children = new System.Collections.Generic.List<Transform>();

        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (child.name.Contains("Piece") || child.name.Contains("Fragment"))
            {
                children.Add(child);
            }
        }

        _shatterPieces = children.ToArray();

        // ������� �������� �����
        if (_mainDrop == null)
        {
            var main = transform.Find("MainDrop");
            if (main != null) _mainDrop = main;
        }

        Debug.Log($"Setup complete: {_shatterPieces.Length} pieces found");
    }
}