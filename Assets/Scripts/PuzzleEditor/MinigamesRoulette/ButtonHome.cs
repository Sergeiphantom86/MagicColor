using System;
using System.Collections;
using PuzzleEditor.Audio;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace PuzzleEditor.MinigamesRoulette
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
        private WaitForSeconds _waitForSeconds;

        public Button Button => _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _voiceover = GetComponent<Voiceover>();
            _menuLoader = GetComponent<MenuLoader>();
            _waitForSeconds = new WaitForSeconds(_audioClip.length);

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
                    YG2.saves.SetCurrency(_coin, _ticket.FullReward);
                }
            }

            StartCoroutine(WaitForWindowClose(() => _menuLoader.TargetScene(Menu)));
        }

        private IEnumerator WaitForWindowClose(Action callback)
        {
            _voiceover.PlayOneShot(_audioClip);

            yield return _waitForSeconds;

            _button.interactable = true;

            callback.Invoke();
        }
    }
}