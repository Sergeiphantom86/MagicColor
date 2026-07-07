using PuzzleResources.MinigamesRoulette;
using UnityEngine;
using Wallets;
using YG;

namespace Menu.Tutorials.TutorialPuzzle
{
    [RequireComponent(typeof(Currency))]

    public class Reward : MonoBehaviour
    {
        [SerializeField] private int _value;

        private Currency _currency;
        private TextAnimator _textAnimator;

        public Currency Currency => _currency;

        private void Awake()
        {
            _currency = GetComponent<Currency>();
            _textAnimator = GetComponentInChildren<TextAnimator>(true);
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
                YG2.saves.SetCurrency(_currency, _textAnimator.Value);
            }
        }
    }
}