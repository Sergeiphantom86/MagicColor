using Game.SaveEditor;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
namespace PuzzleEditor.SoundEditor
{

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
    private IProgressSaver _progressSaver;
    private WaitForSeconds _waitForSeconds;
    private WaitForSeconds _waitStop;

    private void Awake()
    {
        _delay = 2;
        _playbackTime = 0.5f;
        _waitForSeconds = new WaitForSeconds(_delay);
        _waitStop = new WaitForSeconds(_playbackTime);
        _progressSaver = new ProgressSaver();

        SetupAudioSources();
        PlayBackgroundMusic(_progressSaver.Saves.MusicTime);
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

        _progressSaver.SetMusicTime(_musicSource.time);
    }

    private void OnDestroy()
    {
        _progressSaver.SaveProgress();
    }

    public void PlayButtonClick(AudioClip audioClip)
    {
        if (audioClip == null)
            return;

        SetAudioClip(audioClip);

        if (audioClip.length >= _playbackTime)
        {
            StartCoroutine(StopPlaybackAfterWhile());
        }
    }

    private void SetAudioClip(AudioClip audioClip)
    {
        _soundSource.clip = audioClip;
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
        if (_progressSaver.Saves != null)
        {
            _currentMusicVolume = _progressSaver.Saves.MusicVolume;
            _currentSounVolume = _progressSaver.Saves.SoundVolume;
        }
    }

    private void PlayBackgroundMusic(float time)
    {
        if (_backgroundMusic == null || _musicSource.isPlaying)
            return;

        _musicSource.clip = _backgroundMusic;
        _musicSource.time = time;
        _musicSource.Play();
    }

    private void SetVolume(VolumeChanger volumeChanger, float volume)
    {
        UpdateMixerVolume(volumeChanger.name, volume);
        ChangeValuesAudioSource(volumeChanger, volume);
        SaveVolumeSettings();
    }

    private void SaveVolumeSettings()
    {
        if (_coroutineSaving != null)
            return;

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

        _progressSaver.SaveProgress();
        _coroutineSaving = null;
    }
}
}