using UnityEngine;
using YG;

[RequireComponent(typeof(AudioSource))]
public class FootstepSound : MonoBehaviour
{
    [SerializeField] private AudioClip[] _footstepClips;
    [SerializeField] private float _minPitch = 0.9f;
    [SerializeField] private float _maxPitch = 1.1f;


    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        _audioSource.volume = YG2.saves.SoundVolume;
    }

    public void FootstepEvent()
    {
        PlayFootstepSound();
    }

    public (AudioClip clip, float pitch) PlayFootstepSound()
    {
        if (_footstepClips == null || _footstepClips.Length == 0)
        {
            Debug.LogWarning("No footstep clips assigned!");
            return (null, 1f);
        }

        AudioClip clip = _footstepClips[GetRandomIndex()];

        if (_audioSource != null)
        {
            _audioSource.pitch = Random.Range(_minPitch, _maxPitch);

            _audioSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning("AudioSource not found!");
        }

        return (clip, _audioSource.pitch);
    }

    private int GetRandomIndex()
    {
        return Random.Range(0, _footstepClips.Length);
    }
}