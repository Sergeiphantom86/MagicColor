using PuzzleEditor.SoundEditor;
using UnityEngine;

public class VolumeStorage
{
    private float _musicVolume = 0.3f;
    private float _soundVolume = 0.3f;

    public float MusicVolume => _musicVolume;
    public float SoundVolume => _soundVolume;

    public void SetVolume(VolumeChanger volumeChanger, float volume)
    {
        if (volumeChanger == null)
        {
            Debug.LogError("VolumeStorage: VolumeChanger is null");
            return;
        }

        if (float.IsNaN(volume) || float.IsInfinity(volume))
        {
            Debug.LogError($"VolumeStorage: invalid volume value: {volume}");
            return;
        }

        if (volumeChanger is MusicVolumeController)
        {
            _musicVolume = Mathf.Clamp01(volume);
        }
        else if (volumeChanger is VolumeSoundsController)
        {
            _soundVolume = Mathf.Clamp01(volume);
        }
        else
        {
            Debug.LogWarning($"VolumeStorage: unknown VolumeChanger type: {volumeChanger.GetType()}");
        }
    }
}