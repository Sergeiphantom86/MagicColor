using UnityEngine;

public class Ticket : MonoBehaviour
{
    private TextAnimator _textAnimator;
    private Currency _currency;
    private long _fullReward;
    private IProgressSaver _progressSaver;

    public long FullReward => _fullReward;

    private void Awake()
    {
        _currency = GetComponent<Currency>();
        _textAnimator = GetComponentInChildren<TextAnimator>();
        _progressSaver = new ProgressSaver();

        if (_currency == null)
        {
            Debug.LogError("Currency == null");
        }

        if (_textAnimator == null)
        {
            Debug.LogError("TextAnimator == null");
        }
    }

    private void Start()
    {
        Show();
    }

    private void Show()
    {
        _fullReward = _progressSaver.Saves.Reward * _progressSaver.Saves.CountStars;

        _textAnimator.AnimateToValue(_fullReward);
    }
}
