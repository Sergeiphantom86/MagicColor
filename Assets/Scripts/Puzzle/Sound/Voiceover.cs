using UnityEngine;
using UnityEngine.Audio;
using YG;

[RequireComponent(typeof(AudioSource))]
public class Voiceover : MonoBehaviour
{
    private const string SoundVolume = nameof(SoundVolume);
    private const float MinVolume = 0.0001f;
    private const float MaxVolume = 1f;
    private const float DBMultiplier = 20f;

    [SerializeField] private AudioMixerGroup _sfxGroup;

    private AudioSource _sfxSource;

    private void Awake()
    {
        _sfxSource = GetComponent<AudioSource>();
        _sfxSource.outputAudioMixerGroup = _sfxGroup;
        LoadVolumeSettings();
    }

    public void PlaySfx(AudioClip clip)
    {
        if (clip != null && _sfxSource != null)
        {
            _sfxSource.PlayOneShot(clip);
            _sfxSource.time = 0.1f;
        }
    }

    public void Stop()
    {
        _sfxSource?.Stop();
    }

    private void LoadVolumeSettings()
    {
        float clampedVolume = Mathf.Clamp(YG2.saves.SoundVolume, MinVolume, MaxVolume);
        float dbVolume = Mathf.Log10(clampedVolume) * DBMultiplier;
       
        _sfxGroup.audioMixer.SetFloat(SoundVolume, dbVolume);
    }

    private void OnValidate()
    {
        if (_sfxSource != null && _sfxGroup != null)
            _sfxSource.outputAudioMixerGroup = _sfxGroup;
    }
}