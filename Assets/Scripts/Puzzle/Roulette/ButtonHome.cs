using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Voiceover), typeof(Button), typeof(MenuLoader))]
public class ButtonHome : MonoBehaviour
{
    private const string Menu = nameof(Menu);

    [SerializeField] private Warner _warner;
    [SerializeField] private CoinWallet _coin;
    [SerializeField] private AudioClip _audioClip;
    [SerializeField] private AudioClip _errorSound;
    [SerializeField] private CrystalWallet _crystal;
    [SerializeField] private ButtonController _buttonController;
    
    private Voiceover _voiceover;
    private MenuLoader _menuLoader;
    private Button _button;
    private int _extraTime;

    private void Awake()
    {
        _extraTime = 2;
        _button = GetComponent<Button>();
        _voiceover = GetComponent<Voiceover>();
        _menuLoader = GetComponent<MenuLoader>();

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
        _button.onClick.AddListener(Play);
    }

    private void Play()
    {
        _button.interactable = false;

        if (_buttonController != null)
        {
            if (_buttonController.IsSpin == false)
            {
                _warner.TurnOn();

                StartCoroutine(WaitForWindowClose(_errorSound, true, _extraTime, () =>
                    _warner.TurnOff()));

                return;
            }
        }

        StartCoroutine(WaitForWindowClose(_audioClip, true,0, () => 
        _menuLoader.TargetScene(Menu)));
    }

    private IEnumerator WaitForWindowClose(AudioClip clip, bool isOn, int duration, Action callback)
    {
        _voiceover.Play(clip);

        yield return new WaitForSeconds(clip.length + duration);

        _button.interactable = isOn;

        callback.Invoke();
    }
}