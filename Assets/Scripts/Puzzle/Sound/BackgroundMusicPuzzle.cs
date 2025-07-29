using UnityEngine;
using UnityEngine.Audio;
using YG;

public class BackgroundMusicPuzzle : MonoBehaviour
{
    private const string MusicVolume = nameof(MusicVolume);

    [SerializeField] private AudioMixerGroup _musicGroup;

    private AudioSource _musicSource;

    private void Awake()
    {
        _musicSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        SetMixerVolume(MusicVolume, YG2.saves.MusicVolume);

        PlayBackgroundMusic();
    }

    private void PlayBackgroundMusic()
    {
        if (_musicSource == null) return;

        if (_musicSource.isPlaying == false)
            _musicSource.Play();
    }

    private void SetMixerVolume(string parameter, float volume)
    {
        float dbVolume = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1)) * 20;

        _musicGroup.audioMixer.SetFloat(parameter, dbVolume);;
    }

    private void OnValidate()
    {
        if (_musicGroup != null && _musicSource != null)
            _musicSource.outputAudioMixerGroup = _musicGroup;
    }
}
