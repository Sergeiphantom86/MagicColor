using UnityEngine;
using YG;

public class Ticket : MonoBehaviour
{
    private TextAnimator _textAnimator;
    private Currency _currency;
    private long _fullReward;

    public long FullReward => _fullReward;

    private void Awake()
    {
        _currency = GetComponent<Currency>();
        _textAnimator = GetComponentInChildren<TextAnimator>();

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
        _fullReward = _currency.Winn * YG2.saves.CountStars;

        _textAnimator.AnimateToValue(_fullReward);
    }
}
