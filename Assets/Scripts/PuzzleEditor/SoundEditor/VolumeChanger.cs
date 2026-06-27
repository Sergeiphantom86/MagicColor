using System;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace PuzzleEditor.SoundEditor
{
    [RequireComponent(typeof(Slider))]

    public class VolumeChanger : MonoBehaviour
    {
        private ToggleBase _toggleBase;
        private Slider _volumeSlider;
        private float _temporaryVolume;
        private bool _isOn;

        public event Action<VolumeChanger, float> OnVolumeChange;

        private void Awake()
        {
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
                _volumeSlider.value = YG2.saves.MusicVolume;
            }
            else
            {
                _volumeSlider.value = YG2.saves.SoundVolume;
            }
        }

        private void OnEnable()
        {
            _volumeSlider.onValueChanged.AddListener(OnSetVolume);
            _toggleBase.Disabling += ToggleSoundsMute;
        }

        private void OnDisable()
        {
            _volumeSlider.onValueChanged.RemoveListener(OnSetVolume);
            _toggleBase.Disabling -= ToggleSoundsMute;
        }

        private void OnSetVolume(float volume)
        {
            if (_isOn == false && _volumeSlider.value > 0)
            {
                _toggleBase.TurnOn(true);
            }

            OnVolumeChange?.Invoke(this, volume);

            YG2.saves.SetVolume(this, volume);
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