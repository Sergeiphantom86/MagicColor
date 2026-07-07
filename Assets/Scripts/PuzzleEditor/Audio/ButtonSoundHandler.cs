using Menu;
using UnityEngine;

namespace PuzzleEditor.Audio
{
    [RequireComponent(typeof(ButtonCarouselController))]

    public class ButtonSoundHandler : MonoBehaviour
    {
        [SerializeField] private MenuSound _menuSoundManager;

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
            _menuSoundManager.PlayButtonClick(audioClip);
        }
    }
}