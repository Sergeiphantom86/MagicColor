using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class SettingsClosure : MonoBehaviour
{
    [SerializeField] private Window _settingWindow;
    [SerializeField] private ButtonSoundHandler _buttonSound;
    [SerializeField] private AudioClip _audioClip;

    private Button _button;

    public Button Button => _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(() => CloseSettings(_audioClip));
    }

    private void CloseSettings(AudioClip audioClip)
    {
        _buttonSound.PlayButtonSound(audioClip);
        _settingWindow.gameObject.SetActive(false);
    }
}