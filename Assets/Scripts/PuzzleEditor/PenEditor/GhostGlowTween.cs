using UnityEngine;
using DG.Tweening;
namespace PuzzleEditor.PenEditor
{

public class GhostGlowTween : MonoBehaviour
{
    private const string Emission = "_EMISSION";
    private const string EmissionColor = "_EmissionColor";

    [SerializeField] private float _duration;
    [SerializeField] private Gradient _gradient;
    [SerializeField][Range(0f, 1f)] private float _tweenStart;
    [SerializeField][Range(0f, 1f)] private float _tweenEnd;

    private float _emissionMultiplier;
    private Material _material;

    private void Awake()
    {
        _emissionMultiplier = 0.1f;
        _material = GetComponent<Renderer>().material;
        _material.EnableKeyword(Emission);

        if (_material.HasProperty(EmissionColor))
            _material.SetColor(EmissionColor, Color.black);
    }

    private void Start()
    {
        DOTween.To(
            () => 0f,
            time =>
            {
                Color color = _gradient.Evaluate(time) * _emissionMultiplier;
                _material.SetColor(EmissionColor, color);
            },
            1f,
            _duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetTarget(this);
    }
}
}