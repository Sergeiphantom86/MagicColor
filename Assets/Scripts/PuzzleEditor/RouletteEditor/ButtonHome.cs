using System;
using System.Collections;
using Game.SaveEditor;
using PuzzleEditor.SoundEditor;
using UnityEngine;
using UnityEngine.UI;

namespace PuzzleEditor.RouletteEditor
{
    [RequireComponent(typeof(Voiceover), typeof(Button), typeof(MenuLoader))]

    public class ButtonHome : MonoBehaviour
    {
        private const string Menu = nameof(Menu);

        [SerializeField] private Coin _coin;
        [SerializeField] private Ticket _ticket;
        [SerializeField] private AudioClip _audioClip;
        [SerializeField] private ButtonController _buttonController;

        private Voiceover _voiceover;
        private MenuLoader _menuLoader;
        private Button _button;
        private IProgressSaver _progressSaver;

        public Button Button => _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _voiceover = GetComponent<Voiceover>();
            _menuLoader = GetComponent<MenuLoader>();
            _progressSaver = new ProgressSaver();

            if (_button == null)
            {
                Debug.LogError("Button == null");
                return;
            }

            if (_voiceover == null)
            {
                Debug.LogError("Voiceover == null");
                return;
            }

            if (_menuLoader == null)
            {
                Debug.LogError("MenuLoader == null");
                return;
            }
        }

        private void Start()
        {
            _button.onClick.AddListener(GoMenu);
        }

        public void GoMenu()
        {
            _button.interactable = false;

            if (_buttonController != null)
            {
                if (_buttonController.IsSpin == false)
                {
                    _progressSaver.SetCurrency(_coin, _ticket.FullReward);
                }
            }

            StartCoroutine(
            WaitForWindowClose(_audioClip, true, 0, () => _menuLoader.TargetScene(Menu))
            );
        }

        private IEnumerator WaitForWindowClose(
        AudioClip clip,
        bool isOn,
        int duration,
        Action callback
        )
        {
            _voiceover.PlayOneShot(clip);

            yield return new WaitForSeconds(clip.length + duration);

            _button.interactable = isOn;

            callback.Invoke();
        }
    }
}