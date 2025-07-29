using UnityEngine;
using UnityEngine.Audio;
using YG;

public interface IEffectsHandler
{
    public void PlayDragSound();
    public void PlayPixelAppearSound();
    public void PlayWinSound();
    public void PlayCollisionSound();
    public void PlayEndDragging();
    public void PlayTakeSound();
    public void Stop();
}

public class EffectsHandler : MonoBehaviour, IEffectsHandler
{
    private const string SoundVolume = nameof(SoundVolume);
    private const float MinVolume = 0.0001f;
    private const float MaxVolume = 1f;
    private const float DBMultiplier = 20f;

    [SerializeField] private AudioMixerGroup _sfxGroup;

    [Header("Sound Clips")]
    [SerializeField] private AudioClip _dragSound;
    [SerializeField] private AudioClip _pixelAppearSound;
    [SerializeField] private AudioClip _winSound;
    [SerializeField] private AudioClip _collisionSound;
    [SerializeField] private AudioClip _endDragging;
    [SerializeField] private AudioClip _takeSound;

    private AudioSource _sfxSource;

    private void Awake()
    {
        _sfxSource = GetComponent<AudioSource>();
        _sfxSource.outputAudioMixerGroup = _sfxGroup;
        LoadVolumeSettings();
    }

    public void Stop() => 
        _sfxSource.Stop();

    public void PlayDragSound() =>
        PlaySfx(_dragSound);

    public void PlayPixelAppearSound() => 
        PlaySfx(_pixelAppearSound);

    public void PlayWinSound() => 
        PlaySfx(_winSound);

    public void PlayCollisionSound() => 
        PlaySfx(_collisionSound);

    public void PlayEndDragging() =>
        PlaySfx(_endDragging);

    public void PlayTakeSound() => 
        PlaySfx(_takeSound);

    private void PlaySfx(AudioClip clip)
    {
        if (clip != null && _sfxSource != null)
        {
            _sfxSource.PlayOneShot(clip);
        }
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