using UnityEngine;
using DG.Tweening;

public class GhostGlowTween : MonoBehaviour
{
    private const string Emission = "_EMISSION";
    private const string EmissionColor = "_EmissionColor";

    [SerializeField] private float _duration;
    [SerializeField] private Gradient _gradient;
    [SerializeField, Range(0f, 1f)] private float _tweenStart;
    [SerializeField, Range(0f, 1f)] private float _tweenEnd;
    [SerializeField, Range(0f, 1f)] private float emissionMultiplier;

    private Material _material;

    private void Awake()
    {
        _material = GetComponent<Renderer>().material;
        _material.EnableKeyword(Emission);
    }

    private void Start()
    {
        DOTween.To(() => _tweenStart, time =>
        {
            Color color = _gradient.Evaluate(time) * emissionMultiplier;
            _material.SetColor(EmissionColor, color);
        }, _tweenEnd, _duration)
        .SetLoops(-1, LoopType.Yoyo)
        .SetEase(Ease.InOutSine)
        .SetTarget(this);
    }
}