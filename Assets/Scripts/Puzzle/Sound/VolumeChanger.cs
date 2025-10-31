using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeChanger: MonoBehaviour
{
    private ToggleBase _toggleBase;
    private float _temporaryVolume;
    private Slider _volumeSlider;
    private bool _isOn;

    public event Action<VolumeChanger, float> OnVolumeChange;

    private void Awake()
    {
        _volumeSlider = GetComponent<Slider>();
        _toggleBase = GetComponentInChildren<ToggleBase>();

        if (_toggleBase == null)
        {
            Debug.LogError("Slider не назначен!");
            return;
        }

        if (_volumeSlider == null)
        {
            Debug.LogError("Slider не назначен!");
            return;
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