using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using YG;

[RequireComponent(typeof(AudioSource))]
public class MenuSoundManager : MonoBehaviour
{
    private const float MinDecibels = -80f;
    private const float DBLinearRatio = 20f;

    [SerializeField] private AudioClip _backgroundMusic;
    [SerializeField] private AudioMixerGroup _soundMixerGroup;
    [SerializeField] private AudioMixerGroup _musicMixerGroup;
    [SerializeField] private VolumeChanger _musicVolumeChanger;
    [SerializeField] private VolumeChanger _sounVolumeChanger;
    [SerializeField] private AudioMixer _mixer;

    private const float MinVolume = 0.0001f;
    private AudioSource _soundSource;
    private AudioSource _musicSource;
    private float _currentMusicVolume = 1f;
    private float _currentSounVolume = 1f;
    private Coroutine _coroutineSaving;
    private Coroutine _coroutine;

    private void Awake()
    {
        SetupAudioSources();

        LoadVolumeSettings();

        PlayBackgroundMusic();
    }

    private void OnEnable()
    {
        _musicVolumeChanger.OnVolumeChange += SetVolume;
        _sounVolumeChanger.OnVolumeChange += SetVolume;
    }

    private void OnDisable()
    {
        _musicVolumeChanger.OnVolumeChange -= SetVolume;
        _sounVolumeChanger.OnVolumeChange -= SetVolume;
    }

    public void PlayButtonClickSound(AudioClip audioClip)
    {
        if (audioClip == null) return;
        _soundSource.PlayOneShot(audioClip);

        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }

        _coroutine = StartCoroutine(WaitPlaybackFinish(audioClip));
    }

    private void SetupAudioSources()
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        _soundSource = sources[0];

        if (sources.Length < 2)
        {
            _musicSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            _musicSource = sources[1];
        }

        SetAudioSource(_soundSource, isOnLoop: false, _soundMixerGroup, _currentSounVolume);

        SetAudioSource(_musicSource, isOnLoop: true, _musicMixerGroup, _currentMusicVolume);

    }

    private void SetAudioSource(AudioSource audioSource, bool isOnLoop, AudioMixerGroup audioMixerGroup, float volume)
    {
        audioSource.outputAudioMixerGroup = audioMixerGroup;
        audioSource.playOnAwake = false;
        audioSource.loop = isOnLoop;
        audioSource.volume = volume;
        audioSource.mute = true;
    }

    private void LoadVolumeSettings()
    {
        if (YG2.saves != null)
        {
            _currentMusicVolume = YG2.saves.MusicVolume;
            _currentSounVolume = YG2.saves.SoundVolume;
        }
    }

    public void PlayBackgroundMusic()
    {
        if (_backgroundMusic == null || _musicSource.isPlaying) return;

        _musicSource.clip = _backgroundMusic;
        _musicSource.Play();
    }

    private void SetVolume(string slider, float volume)
    {
        UpdateMixerVolume(slider, volume);
        YG2.saves.SetVolume(slider, volume);
        SaveVolumeSettings();
    }

    private void SaveVolumeSettings()
    {
        if (_coroutineSaving == null)
        {
            _coroutineSaving = StartCoroutine(WaitChangeCompleted());
        }
    }

    private void UpdateMixerVolume(string parameter, float volume)
    {
        float volumeDB = volume > MinVolume
            ? Mathf.Log10(volume) * DBLinearRatio
            : MinDecibels;

        _mixer.SetFloat(parameter, volumeDB);
    }

    private IEnumerator WaitChangeCompleted()
    {
        int delay = 2;

        yield return new WaitForSeconds(delay);

        YG2.SaveProgress();
        _coroutineSaving = null;
    }

    private IEnumerator WaitPlaybackFinish(AudioClip audioClip)
    {
        yield return new WaitForSeconds(audioClip.length);
    }
}