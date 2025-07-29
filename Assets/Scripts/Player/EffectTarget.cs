using UnityEngine;
using System.Collections;

public class EffectTarget : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particlePrefab;
    [SerializeField] private float _particleDuration;
    [SerializeField] private AudioClip _dustSound;

    private Coroutine _coroutine;
    private WaitForSeconds _waitForSeconds;
    private AudioSource _audioSource;

    private void Awake()
    {
        _waitForSeconds = new WaitForSeconds(_particleDuration);

        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;

        if (_particlePrefab == null) return;

        _particlePrefab = Instantiate(_particlePrefab, transform.position, Quaternion.identity);
        TurnOff();
    }

    public void PlayDustEffect(Vector3 position)
    {
        if (_particlePrefab != null)
        {
            _audioSource.PlayOneShot(_dustSound);
            _particlePrefab.gameObject.SetActive(false);
            _particlePrefab.transform.position = position;
            TurnOn();
        }
    }

    public void DisableWithDelay()
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }

        _coroutine = StartCoroutine(WaitShutdown());
    }

    private IEnumerator WaitShutdown()
    {
        yield return _waitForSeconds;

        TurnOff();

        _coroutine = null;
    }

    private void TurnOff()
    {
        _particlePrefab.gameObject.SetActive(false);
    }

    private void TurnOn()
    {
        _particlePrefab.gameObject.SetActive(true);
    }
}