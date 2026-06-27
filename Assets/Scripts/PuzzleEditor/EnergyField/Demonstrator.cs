using DG.Tweening;
using UnityEngine;

namespace PuzzleEditor.EnergyField
{
    [RequireComponent(typeof(Renderer))]

    public class Demonstrator : MonoBehaviour
    {
        private const float AnimationEndValue = 1f;

        [SerializeField] private float _duration;
        [SerializeField] private MagicSphere _magicSphere;

        private Tween _pulseTween;
        private Renderer _renderer;
        private Material _startMaterial;
        private Vector3 _startScale;
        private float _maxAlpha;
        private float _timeDivider;
        private float _alphaPeakTime;
        private float _scaleMultiplier;
        private float _scaleBoostThreshold;
        private float _boostedScaleMultiplier;

        private void Awake()
        {
            _duration = 1;
            _maxAlpha = 0.7f;
            _timeDivider = 0.5f;
            _alphaPeakTime = 0.5f;
            _scaleMultiplier = 4;
            _scaleBoostThreshold = 0.7f;
            _boostedScaleMultiplier = 8f;

            _renderer = GetComponent<Renderer>();

            FadeIn(_renderer.material);

            _startScale = transform.localScale;
            _startMaterial = _renderer.material;
        }

        private void Start()
        {
            StartPulsation();
        }

        private void OnDestroy()
        {
            _pulseTween.Kill();
        }

        private void StartPulsation()
        {
            transform.localScale = _startScale;
            SetAlpha(_startMaterial, 0f);

            _pulseTween = DOTween.To(() => 0f,
            time =>
            {
                _scaleMultiplier = GetConfirmatTimeChange(time, _scaleBoostThreshold) ? _scaleMultiplier : _boostedScaleMultiplier;

                transform.localScale = Vector3.Lerp(_startScale, _startScale * _scaleMultiplier, time);

                float alpha = GetConfirmatTimeChange(time, _timeDivider) ? GetMaxAlpha(0, _maxAlpha, time) : GetMaxAlpha(_maxAlpha, 0, time - _alphaPeakTime);

                SetAlpha(_startMaterial, alpha);

            }, AnimationEndValue, _duration)
                .SetEase(Ease.Linear)
                .OnComplete(() => 
                _magicSphere.EnableEndEffect());
        }

        private float GetMaxAlpha(float minAlpha, float maxAlpha, float time)
        {
            return Mathf.Lerp(minAlpha, maxAlpha, time / _timeDivider);
        }

        private bool GetConfirmatTimeChange(float time, float timeLimit)
        {
            return time <= timeLimit;
        }

        private void SetAlpha(Material mat, float alpha)
        {
            Color color = mat.color;
            color.a = alpha;
            mat.color = color;
        }

        private void FadeIn(Material mat)
        {
            mat.SetFloat("_Mode", 3);
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        }
    }
}