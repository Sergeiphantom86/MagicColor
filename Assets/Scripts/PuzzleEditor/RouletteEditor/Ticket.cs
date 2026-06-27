using UnityEngine;
using Wallets;
using YG;

namespace PuzzleEditor.RouletteEditor
{
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
            _fullReward = YG2.saves.Reward * YG2.saves.Stars;

            _textAnimator.AnimateToValue(_fullReward);
        }
    }
}