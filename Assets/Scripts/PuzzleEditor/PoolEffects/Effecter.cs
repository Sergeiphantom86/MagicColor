using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace PuzzleEditor.PoolEffects
{
    public class Effecter : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _prefab;
        [SerializeField] private bool _collectionCheck = true;

        private int _maxPoolSize;
        private int _defaultPoolSize;
        private ParticleSystem _particleSystem;
        private ObjectPool<ParticleSystem> _pool;
        private WaitForSeconds _waitForSeconds;

        private void Awake()
        {
            _maxPoolSize = 50;
            _defaultPoolSize = 10;
            _waitForSeconds = new WaitForSeconds(_prefab.main.duration);

            InitializePools();
        }

        public void CraeteParticles(Vector3 position, Quaternion quaternion, float scale)
        {
            ParticleSystem particles = _pool.Get();

            SetLocation(particles, position, quaternion, scale);

            Return(particles);
        }

        public ParticleSystem CreatePooledItem()
        {
            _particleSystem = Instantiate(_prefab);

            _particleSystem.transform.SetParent(transform);

            _particleSystem.gameObject.SetActive(false);

            return _particleSystem;
        }

        private void InitializePools()
        {
            _pool = new ObjectPool<ParticleSystem>(
            createFunc: CreatePooledItem,
            actionOnGet: OnTakeFromPool,
            actionOnRelease: OnReturnedToPool,
            actionOnDestroy: OnDestroyPoolObject,
            collectionCheck: _collectionCheck,
            defaultCapacity: _defaultPoolSize,
            maxSize: _maxPoolSize
            );
        }

        private void Return(ParticleSystem particles)
        {
            StartCoroutine(ReturnAfterDelay(particles));
        }

        private void OnTakeFromPool(ParticleSystem particles)
        {
            particles.gameObject.SetActive(true);

            if (particles != null)
            {
                if (particles.isPlaying)
                {
                    particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                }

                particles.Play();
            }
        }

        private void SetLocation(
        ParticleSystem particles,
        Vector3 position,
        Quaternion quaternion,
        float scale
        )
        {
            particles.transform.SetPositionAndRotation(position, quaternion);
            particles.transform.localScale = Vector3.one * scale;
        }

        private void OnReturnedToPool(ParticleSystem particles)
        {
            particles.gameObject.SetActive(false);

            if (particles != null)
            {
                particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        private void OnDestroyPoolObject(ParticleSystem particles)
        {
            if (particles != null)
            {
                Destroy(particles);
            }
        }

        private void OnDestroy()
        {
            _pool.Clear();
        }

        private IEnumerator ReturnAfterDelay(ParticleSystem particles)
        {
            yield return _waitForSeconds;

            _pool.Release(particles);
        }
    }
}