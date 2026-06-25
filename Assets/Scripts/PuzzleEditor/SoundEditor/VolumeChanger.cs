using System;
using Game.SaveEditor;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleEditor.SoundEditor
{
    [RequireComponent(typeof(Slider))]
    public class VolumeChanger : MonoBehaviour
    {
        private IProgressSaver _progressSaver;
        private ToggleBase _toggleBase;
        private Slider _volumeSlider;
        private float _temporaryVolume;
        private bool _isOn;

        public event Action<VolumeChanger, float> OnVolumeChange;

        private void Awake()
        {
            _progressSaver = new ProgressSaver();
            _volumeSlider = GetComponent<Slider>();
            _toggleBase = GetComponentInChildren<ToggleBase>();

            if (_toggleBase == null)
            {
                Debug.LogError("Slider �� ��������!");
                return;
            }

            if (_volumeSlider == null)
            {
                Debug.LogError("Slider �� ��������!");
                return;
            }

            if (this is MusicVolumeController)
            {
                _volumeSlider.value = _progressSaver.Saves.MusicVolume;
            }
            else
            {
                _volumeSlider.value = _progressSaver.Saves.SoundVolume;
            }
        }

        private void OnEnable()
        {
            _volumeSlider.onValueChanged.AddListener(SetVolume);
            _toggleBase.OnDisabling += ToggleSoundsMute;
        }

        private void OnDisable()
        {
            _volumeSlider.onValueChanged.RemoveListener(SetVolume);
            _toggleBase.OnDisabling -= ToggleSoundsMute;
        }

        private void SetVolume(float volume)
        {
            if (_isOn == false && _volumeSlider.value > 0)
            {
                _toggleBase.TurnOn(true);
            }

            OnVolumeChange?.Invoke(this, volume);

            _progressSaver.SetVolume(this, volume);
        }

        public void ToggleSoundsMute(bool isOn)
        {
            _isOn = isOn;

            if (isOn == false)
            {
                _temporaryVolume = _volumeSlider.value;
                _volumeSlider.value = _volumeSlider.minValue;
                return;
            }

            _volumeSlider.value = _temporaryVolume;
        }
    }
}