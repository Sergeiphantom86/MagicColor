using UnityEngine;

public class ButtonSoundHandler : MonoBehaviour
{
    [SerializeField] private AudioClip _clickSound;
    [SerializeField] private MenuSoundManager _menuSoundManager;

    private ButtonCarouselController _carouselController;

    private void Awake()
    {
       _carouselController = GetComponent<ButtonCarouselController>();
    }

    private void Start()
    {
        if (_menuSoundManager == null)
        {
            Debug.LogError("MenuSoundManager not found in scene!");
            return;
        }

        if (_carouselController == null)
        {
            Debug.LogError("ButtonCarouselController not found!");
            return;
        }
    }

    public void PlayButtonSound(AudioClip audioClip) 
    {
        _menuSoundManager.PlayButtonClickSound(audioClip);
    }
}