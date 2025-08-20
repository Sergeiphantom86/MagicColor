using System;
using UnityEngine;
using UnityEngine.UI;
using YG;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(Slider))]
public class VolumeChanger: MonoBehaviour
{
    [SerializeField] private ToggleBase _toggleBase;
    private float _temporaryVolume;
    private Slider _volumeSlider;
    private bool _isOn;

    public event Action<string, float> OnVolumeChange;

    private void Awake()
    {
        _volumeSlider = GetComponent<Slider>();

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
        
        OnVolumeChange?.Invoke(_volumeSlider.name, volume);
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