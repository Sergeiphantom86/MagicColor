using DG.Tweening;
using PuzzleEditor.PoolEffects;
using PuzzleEditor.Audio;
using UnityEngine;

namespace PuzzleEditor.Spawners
{
    public class SpawnDropAnimation : MonoBehaviour
    {
        [SerializeField] private float _startYOffset;
        [SerializeField] private float _duration;
        [SerializeField] private Ease _ease = Ease.OutCubic;
        [SerializeField] private AudioClip _fall;

        private Tween _tween;
        private Vector3 _targetWorldPosition;
        private ColorableObject _targetColor;
        private Collider _collider;
        private Voiceover _voiceover;
        private float _valueTransparency;

        public float Duration => _duration;

        private void Awake()
        {
            _targetColor = GetComponent<ColorableObject>();
            _collider = GetComponent<Collider>();
            _voiceover = GetComponent<Voiceover>();
            _valueTransparency = 0.6f;
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _tween?.Play();

            if (_voiceover != null && _fall != null)
            _voiceover.PlayOneShot(_fall);
        }

        public void Create(Effecter effecter)
        {
            SetTargetPosition();

            SetStartPosition();

            _tween?.Kill();

            _collider.enabled = true;

            _tween = transform
            .DOMove(_targetWorldPosition, _duration)
            .OnComplete(() =>
            {
                effecter.CraeteParticles(transform.position, Quaternion.identity, 0.5f);

                _targetColor.SetAlpha(_valueTransparency);
                })
                .SetEase(_ease)
                .Pause();
            }

            private void SetStartPosition()
            {
                transform.position = _targetWorldPosition + Vector3.up * _startYOffset;
            }

            private void SetTargetPosition()
            {
                _targetWorldPosition = transform.position;
            }
        }
    }