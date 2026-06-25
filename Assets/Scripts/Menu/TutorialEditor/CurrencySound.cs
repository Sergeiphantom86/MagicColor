using PuzzleEditor.SoundEditor;
using UnityEngine;

namespace Menu.TutorialEditor
{
    public class CurrencySound : MonoBehaviour
    {
        [SerializeField]
        private AudioClip _audioClip;

        private Voiceover _voiceover;

        private void Awake()
        {
            _voiceover = GetComponent<Voiceover>();
        }

        private void OnEnable()
        {
            if (_voiceover != null)
            {
                _voiceover.PlayOneShot(_audioClip);
            }
        }
    }
}