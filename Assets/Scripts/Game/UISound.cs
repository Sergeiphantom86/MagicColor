using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using YG;

public class UISound : MonoBehaviour
{
    private const string SoundVolume = nameof(SoundVolume);

    [SerializeField] private AudioMixerGroup _sfxGroup;

    [Header("Sound Clips")]
    [SerializeField] private AudioClip _click;
    [SerializeField] private AudioClip _teleport;
    [SerializeField] private AudioClip _panelAnimation;

    private AudioSource _sfxSource;

    private void Awake()
    {
        _sfxSource = GetComponent<AudioSource>();

        _sfxSource.outputAudioMixerGroup = _sfxGroup;
    }

    private void Start()
    {
        LoadVolumeSettings();
    }

    public void Stop()
    {
        _sfxSource.Stop();
    }

    public void OnClick()
    {
        PlaySfx(_click);

        StartCoroutine(WaitForAudioFinish(_sfxSource));
    }

    public void OnTeleport()
    {
        PlaySfx(_teleport);
    }

    public void OnPanelAnimation()
    {
        PlaySfx(_panelAnimation);
    }

    private void PlaySfx(AudioClip clip)
    {
        if (clip != null && _sfxSource != null)
        {
            _sfxSource.PlayOneShot(clip);
        }
    }

    private void LoadVolumeSettings()
    {
        float dbVolume = Mathf.Log10(Mathf.Clamp(YG2.saves.SoundVolume, 0.0001f, 1)) * 20;

        _sfxGroup.audioMixer.SetFloat(SoundVolume, dbVolume);
    }

    private void OnValidate()
    {
        if (_sfxGroup != null && _sfxSource != null)
            _sfxSource.outputAudioMixerGroup = _sfxGroup;
    }

    private IEnumerator WaitForAudioFinish(AudioSource audioSource)
    {
        if (audioSource.clip == null) yield break;

        yield return new WaitForSeconds(audioSource.clip.length - audioSource.time);

        audioSource.Stop();
    }
}
