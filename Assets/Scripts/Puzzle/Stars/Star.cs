using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image), typeof(Voiceover))]
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
        if (_image == null)
        {
            Debug.LogError("Image component is null in Star");
            return;
        }

        if (_voiceover == null)
        {
            Debug.LogError("Voiceover component is null in Star");
            return;
        }

        _image.enabled = isOn;

        if (isOn && _audioClip != null)
        {
            _voiceover.Play(_audioClip);
        }
    }
}