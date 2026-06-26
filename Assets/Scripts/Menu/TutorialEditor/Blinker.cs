using System;
using DG.Tweening;
using PuzzleEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Menu.TutorialEditor
{
    public class Blinker : MonoBehaviour, IActivatable
    {
        [SerializeField] private Button _button;

        private float _minAlpha;
        private float _maxAlpha;
        private float _blinkSpeed;
        private Tween _blinkTween;
        private Image _targetImage;
        private Color _originalColor;

        public event Action Completed;

        private void Awake()
        {
            _minAlpha = 0f;
            _maxAlpha = 1f;
            _blinkSpeed = 0.5f;

            _targetImage = GetComponent<Image>();

            if (_targetImage == null)
            {
                Debug.LogError("Blinker: ��� ���������� Image �� ������� " + gameObject.name);
                enabled = false;
                return;
            }

            _originalColor = _targetImage.color;

            Deactivate();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnGoNext);
        }

        private void OnDisable()
        {
            _blinkTween?.Kill();
        }

        public void Play()
        {
            SetAlpha(_maxAlpha);
            _blinkTween = _targetImage
            .DOFade(_minAlpha, _blinkSpeed)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetUpdate(true);
        }

        private void OnGoNext()
        {
            Completed?.Invoke();
            Stop();
        }

        public void Stop()
        {
            if (_blinkTween != null && _blinkTween.IsActive())
            {
                _blinkTween.Kill();
                _blinkTween = null;
                SetAlpha(_minAlpha);
            }

            Deactivate();
        }

        private void SetAlpha(float alpha)
        {
            Color startColor = _originalColor;
            startColor.a = alpha;
            _targetImage.color = startColor;
        }

        public void Activate()
        {
            gameObject.SetActive(true);
            Play();
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}