using Game.SaveEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace PuzzleEditor.SoundEditor
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
        private IProgressSaver _progressSaver;
        private float _volumeDB;

        private void Awake()
        {
            _musicSource = GetComponent<AudioSource>();
            _progressSaver = new ProgressSaver();

            if (_musicSource == null)
            {
                Debug.LogError("AudioSource == null");
            }

            _musicSource.outputAudioMixerGroup = _musicGroup;
        }

        private void Start()
        {
            UpdateMixerVolume(MusicVolume, _progressSaver.Saves.MusicVolume);

            PlayBackgroundMusic(_progressSaver.Saves.MusicTime);
        }

        private void OnDisable()
        {
            _progressSaver.SetMusicTime(_musicSource.time);
        }

        private void OnDestroy()
        {
            _progressSaver.SaveProgress();
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

        private void OnValidate()
        {
            if (_musicGroup != null && _musicSource != null)
            _musicSource.outputAudioMixerGroup = _musicGroup;
        }
    }
}