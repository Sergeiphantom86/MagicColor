using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class Voiceover : MonoBehaviour
{
    private const string SoundVolume = nameof(SoundVolume);
    private const float MinVolume = 0.0001f;
    private const float MaxVolume = 1f;
    private const float DBMultiplier = 20f;

    [SerializeField] private AudioMixerGroup _sfxGroup;

    private AudioSource _sfxSource;
    private float _currentVolume;
    private IProgressSaver _progressSaver;

    public bool IsPlaying => _sfxSource.isPlaying;

    private void Awake()
    {
        _sfxSource = GetComponent<AudioSource>();
        _progressSaver = new ProgressSaver();
        _sfxSource.outputAudioMixerGroup = _sfxGroup;

        LoadVolumeSettings();
    }

    public void PlayOneShot(AudioClip clip)
    {
        if (clip == false || _sfxSource == false || _sfxSource.enabled == false || gameObject.activeInHierarchy == false) 
            return;

        _sfxSource.PlayOneShot(clip);
    }

    public void Play(AudioClip clip)
    {
        _sfxSource.clip = clip;
        _sfxSource.Play();
    }

    public void Stop()
    {
        _sfxSource.Stop();
    }

    public void SetVolume(float vfd)
    {
        float dbVolume = Mathf.Log10(_progressSaver.Saves.SoundVolume * vfd) * DBMultiplier;

        _sfxGroup.audioMixer.SetFloat(SoundVolume, dbVolume);
    }

    private void LoadVolumeSettings()
    {
        float clampedVolume = Mathf.Clamp(_progressSaver.Saves.SoundVolume, MinVolume, MaxVolume);
        float dbVolume = Mathf.Log10(clampedVolume) * DBMultiplier;

        if (_sfxGroup != null)
        {
            _sfxGroup.audioMixer.SetFloat(SoundVolume, dbVolume);
            _currentVolume = dbVolume;
        }
    }

    private void OnValidate()
    {
        if (_sfxSource != null && _sfxGroup != null)
            _sfxSource.outputAudioMixerGroup = _sfxGroup;
    }
}