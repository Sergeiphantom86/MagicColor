using CartoonFX;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class ParticleSystemPool : MonoBehaviour
{
    [SerializeField] private ParticleSystem _prefab;
    [SerializeField] private bool _collectionCheck = true;

    private int _maxPoolSize;
    private int _defaultPoolSize;
    private ParticleSystem _particleSystem;
    private ObjectPool<ParticleSystem> _pool;
    private CFXR_Effect _cFXR_Effect;

    public ObjectPool<ParticleSystem> Pool => _pool;

    private void Awake()
    {
        _maxPoolSize = 50;
        _defaultPoolSize = 10;
        _cFXR_Effect = GetComponent<CFXR_Effect>();
        InitializePools();
    }

    public void InitializePools()
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

    public void Return(ParticleSystem particleSystem)
    {
        StartCoroutine(ReturnAfterDelay(particleSystem));
    }

    public void CreateEffect(ParticleSystem particleSystem)
    {

        if (_cFXR_Effect != null)
        {
            _cFXR_Effect.ResetState();
            if (_cFXR_Effect.cameraShake != null)
            {
                _cFXR_Effect.cameraShake.FetchCameras();
                _cFXR_Effect.cameraShake.StartShake();
            }
        }
    }

    private ParticleSystem CreatePooledItem()
    {
        _particleSystem = Instantiate(_prefab);

        if (_particleSystem.transform.parent != null)
        {
            _particleSystem.transform.SetParent(_particleSystem.transform.parent);
        }
        else
        {
            _particleSystem.transform.SetParent(transform);
        }

        _particleSystem.gameObject.SetActive(false);

        return _particleSystem;
    }

    private void OnTakeFromPool(ParticleSystem particleSystem)
    {
        particleSystem.gameObject.SetActive(true);

        if (particleSystem != null)
        {
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particleSystem.Play();
        }

        if (_cFXR_Effect != null)
        {
            _cFXR_Effect.ResetState();
        }
    }

    private void OnReturnedToPool(ParticleSystem particleSystem)
    {
        particleSystem.gameObject.SetActive(false);

        if (particleSystem != null)
        {
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }

    private void OnDestroyPoolObject(ParticleSystem particleSystem)
    {
        if (particleSystem != null)
        {
            Destroy(particleSystem);
        }
    }

    private void OnDestroy()
    {
        _pool.Clear();
    }

    private IEnumerator ReturnAfterDelay(ParticleSystem effect)
    {
        yield return new WaitForSeconds(effect.main.duration);
        _pool.Release(effect);
    }
}