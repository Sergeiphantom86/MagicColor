using System.Collections;
using PuzzleEditor.SoundEditor;
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

        private void Awake()
        {
            _minLaunchDelay = 1;
            _maxLaunchDelay = 3;
            _audioSource = GetComponent<Voiceover>();
            _particleSystem = GetComponent<ParticleSystem>();
            _waitSoundGlow = new WaitForSeconds(GlowDelay);
            _waitSoundExplosion = new WaitForSeconds(ExplosionToSparkleDelay);

            _audioSource.SetVolume(2);
        }

        public void StartFireworks()
        {
            if (_isRunning)
                return;

            _isRunning = true;

            _particleSystem.Play();

            TryStopCoroutine(_launchCoroutine);

            _launchCoroutine = StartCoroutine(FireworksRoutine());
        }

        public void Stop()
        {
            gameObject.SetActive(false);
        }

        public void StopFireworks()
        {
            _isRunning = false;

            _particleSystem.Stop();

            TryStopCoroutine(_launchCoroutine);
            TryStopCoroutine(_explosioCoroutine);

            _audioSource.Stop();
        }

        private void TryStopCoroutine(Coroutine coroutine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }

        private IEnumerator FireworksRoutine()
        {
            while (_isRunning && _particleSystem.isPlaying)
            {
                TryStopCoroutine(_explosioCoroutine);

                yield return _explosioCoroutine = StartCoroutine(PlaySingleFirework());
                yield return new WaitForSeconds(GetAccidentalDelay());
            }
        }

        private float GetAccidentalDelay()
        {
            return Random.Range(_minLaunchDelay, _maxLaunchDelay);
        }

        private IEnumerator PlaySingleFirework()
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
                yield return new WaitForSeconds(Random.Range(SparkleMinDelay, SparkleMaxDelay));
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

        private void OnDisable()
        {
            StopFireworks();
        }

        private void OnDestroy()
        {
            TryStopCoroutine(_launchCoroutine);
            TryStopCoroutine(_explosioCoroutine);
        }
    }
}