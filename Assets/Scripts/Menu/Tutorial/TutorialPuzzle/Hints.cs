using UnityEngine;

[RequireComponent(typeof(Voiceover))]
public class Hints : MonoBehaviour
{
    [SerializeField] private AudioClip _audioClip;

    private TextSwitcher _textSwitcher;
    private Voiceover _voiceover;

    private void Awake()
    {
        _textSwitcher = GetComponentInChildren<TextSwitcher>();
        _voiceover = GetComponent<Voiceover>();
    }

    public void TurnOn(bool isOn)
    {
        gameObject.SetActive(true);
        _voiceover.PlayOneShot(_audioClip);
        _textSwitcher.TurnOffDesiredOne(isOn);
    }

    public void TurnOff()
    {
        gameObject.SetActive(false);
    }
}