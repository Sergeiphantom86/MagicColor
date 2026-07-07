using System.Collections;
using PuzzleResources.Audio;
using UnityEngine;

namespace Fireworks
{
    [RequireComponent(typeof(ParticleSystem))]

    public class SimpleFireworksAudio : MonoBehaviour
    {
        private const float GlowDelay = 0.2f;
        private const float SparkleCount = 1;
        private const float SparkleMinDelay = 0.05f;
        private const float SparkleMaxDelay = 0.1f;
        private const float ExplosionToSparkleDelay = 0.1f;

        [SerializeField] private FireworksSoundPack _soundPack;

        private bool _isRunning;
        private float _minLaunchDelay;
        private float _maxLaunchDelay;
        private Voiceover _audioSource;
        private Coroutine _launchCoroutine;
        private Coroutine _explosioCoroutine;
        private ParticleSystem _particleSystem;
        private WaitForSeconds _waitSoundGlow;
        private WaitForSeconds _waitSoundExplosion;
        private WaitForSeconds _additionalDelayBetweenShots;
        private WaitForSeconds _additionalDelayBetweenFlashes;

        private void Awake()
        {
            _minLaunchDelay = 1;
            _maxLaunchDelay = 3;
            _audioSource = GetComponent<Voiceover>();
            _particleSystem = GetComponent<ParticleSystem>();
            _waitSoundGlow = new WaitForSeconds(GlowDelay);
            _waitSoundExplosion = new WaitForSeconds(ExplosionToSparkleDelay);
            _additionalDelayBetweenShots = new WaitForSeconds(GetAccidentalDelay(_minLaunchDelay, _maxLaunchDelay));
            _additionalDelayBetweenFlashes = new WaitForSeconds(GetAccidentalDelay(SparkleMinDelay, SparkleMaxDelay));
            _audioSource.SetVolume(2);
        }

        private void OnDisable()
        {
            StopFireworks();
        }

        private void OnDestroy()
        {
            StopDesiredCoroutine(_launchCoroutine);
            StopDesiredCoroutine(_explosioCoroutine);
        }

        public void StartFireworks()
        {
            if (_isRunning)
                return;

            _isRunning = true;

            _particleSystem.Play();

            StopDesiredCoroutine(_launchCoroutine);

            _launchCoroutine = StartCoroutine(FireVolley());
        }

        public void Stop()
        {
            gameObject.SetActive(false);
        }

        public void StopFireworks()
        {
            _isRunning = false;

            _particleSystem.Stop();

            StopDesiredCoroutine(_launchCoroutine);
            StopDesiredCoroutine(_explosioCoroutine);

            _audioSource.Stop();
        }

        private void StopDesiredCoroutine(Coroutine coroutine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }

        private IEnumerator FireVolley()
        {
            while (_isRunning && _particleSystem.isPlaying)
            {
                StopDesiredCoroutine(_explosioCoroutine);

                yield return _explosioCoroutine = StartCoroutine(FireSingle());
                yield return _additionalDelayBetweenShots;
            }
        }

        private float GetAccidentalDelay(float min, float max)
        {
            return Random.Range(min, max);
        }

        private IEnumerator FireSingle()
        {
            if (_soundPack == null)
                yield break;

            yield return _waitSoundGlow;
            yield return _waitSoundGlow;

            PlaySoundWithRandomPitch(_soundPack.ExplosionSound);

            yield return _waitSoundExplosion;

            for (int i = 0; i < SparkleCount; i++)
            {
                PlaySoundWithRandomPitch(_soundPack.SparkleSound);
                yield return _additionalDelayBetweenFlashes;
            }

            yield return _waitSoundGlow;

            PlaySoundWithRandomPitch(_soundPack.GlowSound);
        }

        private void PlaySoundWithRandomPitch(AudioClip clip)
        {
            if (clip != null)
            {
                _audioSource.Play(clip);
            }
        }
    }
}