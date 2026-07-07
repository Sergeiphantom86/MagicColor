using PuzzleResources.Audio;
using UnityEngine;

namespace Menu.Tutorials.TutorialPuzzle
{
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

        public void EnableEffects(bool isOn)
        {
            TurnOn();
            _voiceover.PlayOneShot(_audioClip);
            _textSwitcher.TurnOffDesiredOne(isOn);
        }

        public void TurnOff()
        {
            gameObject.SetActive(false);
        }

        public void TurnOn()
        {
            gameObject.SetActive(true);
        }
    }
}