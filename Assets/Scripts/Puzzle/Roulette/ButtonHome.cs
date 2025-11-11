using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

[RequireComponent(typeof(Voiceover), typeof(Button))]
public class ButtonHome : MonoBehaviour
{
    private const string Menu = nameof(Menu);

    [SerializeField] private Warner _warner;
    [SerializeField] private CoinWallet _coin;
    [SerializeField] private AudioClip _audioClip;
    [SerializeField] private AudioClip _errorSound;
    [SerializeField] private CrystalWallet _crystal;
    [SerializeField] private ButtonController _buttonController;
    [SerializeField] private ErrorPanel _errorPanel;

    private Voiceover _voiceover;
    private Button _button;
    private int _extraTime;

    private void Awake()
    {
        _extraTime = 2;
        _voiceover = GetComponent<Voiceover>();
        _button = GetComponent<Button>();
    }

    private void Start()
    {
        _button.onClick.AddListener(Play);
    }

    private void Play()
    {
        if (_buttonController.IsSpin == false)
        {
            _warner.TurnOn();
            _errorPanel.TurnOn();
            _button.interactable = false;
            StartCoroutine(WaitForWindowClose(_errorSound, true, _extraTime, () =>
                _warner.TurnOff()));

            return;
        }

        _button.interactable = false;
        StartCoroutine(WaitForWindowClose(_audioClip, true,0, () =>
        LoadTargetScene()));
    }

    private void LoadTargetScene()
    {
        YG2.saves.SetAssembledPuzzle(false);

        if (SceneLoader.Instance == null)
        {
            Debug.LogError("SceneLoader instance not found! Using default load.");
            SceneManager.LoadScene("Menu");
            return;
        }

        if (_coin != null && _crystal != null)
        {
            SaveProgress();
        }

        SceneLoader.Instance.LoadSceneWithSplash(Menu);
    }

    private void SaveProgress()
    {
        YG2.saves.SetCurrency(_coin, _coin.Balance);
        YG2.saves.SetCurrency(_crystal, _crystal.Balance);
        YG2.saves.SetAssembledPuzzle(true);
        YG2.saves.ResetSprite();
        YG2.SaveProgress();
    }

    private IEnumerator WaitForWindowClose(AudioClip clip, bool isOn, int duration, Action callback)
    {
        _voiceover.PlaySfx(clip);
       
        yield return new WaitForSeconds(clip.length + duration);

        _button.interactable = isOn;

        callback.Invoke();
    }
}