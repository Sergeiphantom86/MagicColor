public class SoundSaver
{
    private const string MusicVolume = nameof(MusicVolume);
    private float _musicVolume = 0.8f;
    private float _soundVolume = 0.5f;
    private bool _musicToggle = true;
    private bool _soundToggle = true;


    public void SetVolume(string name, float volume)
    {
        if (nameof(MusicVolume) == name)
        {
            _musicVolume = volume;
        }
        else
        {
            _soundVolume = volume;
        }
    }

    public float GetVolume(string name)
    {
        if (nameof(MusicVolume) == name)
        {
            return _musicVolume;
        }
        else
        {
            return _soundVolume;
        }
    }

    public void SetValueToggle(ToggleBase toggleBase, bool isOn)
    {
        if (toggleBase is ToggleMusic)
        {
            _musicToggle = isOn;
            return;
        }

        _soundToggle = isOn;
    }

    public bool GetValueToggle(ToggleBase toggleBase)
    {
        if (toggleBase is ToggleMusic)
        {
            return _musicToggle;
        }

        return _soundToggle;
    }
}