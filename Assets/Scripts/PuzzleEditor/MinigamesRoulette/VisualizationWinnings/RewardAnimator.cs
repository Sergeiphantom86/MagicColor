using Menu.Tutorials.TutorialPuzzle;
using PuzzleEditor.Audio;
using UnityEngine;
using UnityEngine.UI;
using Wallets;
using YG;

namespace PuzzleEditor.MinigamesRoulette.VisualizationWinnings
{
    public class RewardAnimator : MonoBehaviour
    {
        [SerializeField] private GameObject _spritePrefab;
        [SerializeField] private Rewards _rewards;
        [SerializeField] private CoinWallet _coinWallet;
        [SerializeField] private CrystalWallet _crystalWallet;
        [SerializeField] private float _explosionRadius;
        [SerializeField] private float _minScale;
        [SerializeField] private float _maxScale;
        [SerializeField] private float _firstPhaseRatio;
        [SerializeField] private AudioClip _audioClip;

        private float _duration;
        private Vector2 _directionRange;
        private float _directionValueY;
        private int _quantityMultiplier;
        private float _moveToTargetDuration;
        private float _explosionDistanceMultiplier;
        private Voiceover _voiceover;

        private void Awake()
        {
            _duration = 0.2f;
            _directionValueY = 1f;
            _quantityMultiplier = 5;
            _moveToTargetDuration = 0.5f;
            _explosionDistanceMultiplier = 20f;
            _directionRange = new Vector2(-2f, 2f);
            _voiceover = GetComponent<Voiceover>();
        }

        public void ActivateAtPosition(Currency item)
        {
            int particleCount = GetNumberParticles(item);

            _voiceover.PlayOneShot(_audioClip);

            for (int i = 0; i < particleCount; i++)
            {
                CreateParticle(item);
            }

            HandleParticleComplete(item);
        }

        private int GetNumberParticles(Currency item)
        {
            return item is Crystal
            ? item.Value / _quantityMultiplier
            : item.Value * _quantityMultiplier;
        }

        private void CreateParticle(Currency item)
        {
            GameObject particle = Instantiate(
                _spritePrefab,
                transform.position,
                Quaternion.Euler(0, 0, Random.Range(0, 360f)),
                transform);

            particle.transform.localScale = Vector3.zero;

            if (particle.TryGetComponent(out Image image))
            {
                image.sprite = item.Icon.sprite;
            }

            ParticleAnimation anim = particle.AddComponent<ParticleAnimation>();

            anim.Initialize(CalculateRandomPosition(), _rewards.transform.position, GetParticleAnimation());
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

        private void HandleParticleComplete(Currency item)
        {
            _rewards.Appoint(item, GetAward(item));
        }

        private int GetAward(Currency item)
        {
            if (item is Crystal)
            {
                return item.Value;
            }
            else
            {
                return item.Value * YG2.saves.Reward * YG2.saves.Stars;
            }
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
}