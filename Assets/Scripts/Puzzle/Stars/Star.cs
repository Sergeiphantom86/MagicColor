using UnityEngine;
using UnityEngine.UI;

public class Star : MonoBehaviour
{
    [SerializeField] private AudioClip _audioClip;

    private Image _image;
    private Voiceover _voiceover;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _voiceover = GetComponent<Voiceover>();
    }

    public void SetActive(bool isOn)
    {
        _image.enabled = isOn;

        if (isOn)
        {
            _voiceover.PlaySfx(_audioClip);
        }
    }
}