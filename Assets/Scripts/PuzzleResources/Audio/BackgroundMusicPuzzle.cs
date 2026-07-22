using UnityEngine;
using UnityEngine.Audio;
using YG;

namespace PuzzleResources.Audio
{
    [RequireComponent(typeof(AudioSource))]

    public class BackgroundMusicPuzzle : MonoBehaviour
    {
        private const string MusicVolume = nameof(MusicVolume);
        private const float MinDecibels = -80f;
        private const float DBLinearRatio = 20f;
        private const float MinVolume = 0.0001f;

        [SerializeField] private AudioMixerGroup _musicGroup;
        [SerializeField] private AudioClip _backgroundMusic;

        private AudioSource _musicSource;
        private float _volumeDB;

        private void Awake()
        {
            _musicSource = GetComponent<AudioSource>();

            if (_musicSource == null)
            {
                Debug.LogError("AudioSource == null");
            }

            _musicSource.outputAudioMixerGroup = _musicGroup;
        }

        private void Start()
        {
            UpdateMixerVolume(MusicVolume, YG2.saves.MusicVolume);

            PlayBackgroundMusic(YG2.saves.MusicTime);
        }

        private void OnDisable()
        {
            YG2.saves.MusicTime = _musicSource.time;
        }

        private void OnDestroy()
        {
            YG2.SaveProgress();
        }

        private void OnValidate()
        {
            if (_musicGroup != null && _musicSource != null)
                _musicSource.outputAudioMixerGroup = _musicGroup;
        }

        private void PlayBackgroundMusic(float time)
        {
            if (_backgroundMusic == null || _musicSource.isPlaying)
            return;

            _musicSource.clip = _backgroundMusic;
            _musicSource.time = time;
            _musicSource.Play();
        }

        private void UpdateMixerVolume(string nameSlider, float volume)
        {
            _volumeDB = volume > MinVolume ? Mathf.Log10(volume) * DBLinearRatio : MinDecibels;

            _musicGroup.audioMixer.SetFloat(nameSlider, _volumeDB);
        }
    }
}