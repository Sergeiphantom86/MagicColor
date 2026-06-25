using Game.SaveEditor;
using PuzzleEditor.RouletteEditor;
using UnityEngine;
using Wallets;
namespace Menu.TutorialEditor.TutorialPuzzle
{

[RequireComponent(typeof(Currency))]
public class Reward : MonoBehaviour
{
    [SerializeField] private int _value;

    private Currency _currency;
    private TextAnimator _textAnimator;
    private IProgressSaver _progressSaver;

    public Currency Currency => _currency;

    private void Awake()
    {
        _currency = GetComponent<Currency>();
        _textAnimator = GetComponentInChildren<TextAnimator>(true);
        _progressSaver = new ProgressSaver();
    }

    public void SetValue(int value)
    {
        _value += value;
    }

    public void Show()
    {
        if (_textAnimator != null && _value > 0)
        {
            _textAnimator.AnimateToValue(_value);
        }
    }

    public void Save()
    {
        if (_currency != null)
        {
            _progressSaver.SetCurrency(_currency, _textAnimator.Value);
        }
    }
}
}