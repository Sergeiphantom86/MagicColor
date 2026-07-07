using PuzzleEditor.Stars;
using UnityEngine;

namespace Menu.Tutorials.TutorialUI
{
    public class TimerFringe : Fring
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private StarsCounter _starsCounter;

        public StarsCounter StarsCounter => _starsCounter;

        private void OnEnable()
        {
            _panel.SetActive(true);
        }

        private void OnDisable()
        {
            _panel.SetActive(false);
        }
    }
}