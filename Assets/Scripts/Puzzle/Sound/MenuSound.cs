using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using YG;

[RequireComponent(typeof(AudioSource))]
public class MenuSound : MonoBehaviour
{
    private const float MinDecibels = -80f;
    private const float DBLinearRatio = 20f;
    private const float MinVolume = 0.0001f;

    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private AudioClip _backgroundMusic;
    [SerializeField] private AudioMixerGroup _soundMixerGroup;
    [SerializeField] private AudioMixerGroup _musicMixerGroup;
    [SerializeField] private MusicVolumeController _musicVolumeChanger;
    [SerializeField] private VolumeSoundsController _sounVolumeChanger;

    private AudioSource _soundSource;
    private AudioSource _musicSource;
    private Coroutine _coroutineSaving;
    private float _currentMusicVolume;
    private float _currentSounVolume;
    private float _volumeDB;
    private int _delay;
    private float _playbackTime;
    private float _startPlayback;
    private WaitForSeconds _waitForSeconds;
    private WaitForSeconds _waitStop;

    private void Awake()
    {
        _delay = 2;
        _playbackTime = 0.5f;
        _startPlayback = 0.03f;
        _waitForSeconds = new WaitForSeconds(_delay);
        _waitStop = new WaitForSeconds(_playbackTime);
        _playbackTime = 0.5f;
        SetupAudioSources();
        PlayBackgroundMusic(YG2.saves.MusicPlaybackTime);

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

        YG2.saves.SetMusicPlaybackTime(_musicSource.time);
    }

    public void PlayButtonClick(AudioClip audioClip)
    {
        if (audioClip == null) return;

        SetAudioClip(audioClip);

        if (audioClip.length >= _playbackTime)
        {
            StartCoroutine(StopPlaybackAfterWhile());
        }
    }

    private void SetAudioClip(AudioClip audioClip)
    {
        _soundSource.clip = audioClip;
        _soundSource.time = _startPlayback;
        _soundSource.Play();
    }

    private IEnumerator StopPlaybackAfterWhile()
    {
        yield return _waitStop;
        _soundSource.Stop();
    }

    private void SetupAudioSources()
    {
        LoadVolumeSettings();

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

        SetAudioSource(_soundSource, isOn: false, _soundMixerGroup, _currentSounVolume);

        SetAudioSource(_musicSource, isOn: true, _musicMixerGroup, _currentMusicVolume);

    }

    private void SetAudioSource(AudioSource audioSource, bool isOn, AudioMixerGroup audioMixerGroup, float volume)
    {
        audioSource.outputAudioMixerGroup = audioMixerGroup;
        audioSource.playOnAwake = false;
        audioSource.loop = isOn;
        audioSource.volume = volume;
    }

    private void LoadVolumeSettings()
    {
        if (YG2.saves != null)
        {
            _currentMusicVolume = YG2.saves.MusicVolume;
            _currentSounVolume = YG2.saves.SoundVolume;
        }
    }

    public void PlayBackgroundMusic(float time)
    {
        if (_backgroundMusic == null || _musicSource.isPlaying) return;

        _musicSource.clip = _backgroundMusic;
        _musicSource.Play();
        _musicSource.time = time;
    }

    private void SetVolume(VolumeChanger volumeChanger, float volume)
    {
        UpdateMixerVolume(volumeChanger.name, volume);
        ChangeValuesAudioSource(volumeChanger, volume);
        SaveVolumeSettings();
    }

    private void SaveVolumeSettings()
    {
        if (_coroutineSaving != null) return;

        _coroutineSaving = StartCoroutine(WaitChangeCompleted());

    }

    private void UpdateMixerVolume(string nameSlider, float volume)
    {
        _volumeDB = volume > MinVolume
            ? Mathf.Log10(volume) * DBLinearRatio
            : MinDecibels;

        _mixer.SetFloat(nameSlider, _volumeDB);
    }

    private void ChangeValuesAudioSource(VolumeChanger volumeChanger, float volume)
    {
        if (volumeChanger is VolumeSoundsController)
        {
            _soundSource.volume = volume;
            
            return;
        }

        _musicSource.volume = volume;
    }

    private IEnumerator WaitChangeCompleted()
    {
        yield return _waitForSeconds;

        YG2.SaveProgress();
        _coroutineSaving = null;
    }
}