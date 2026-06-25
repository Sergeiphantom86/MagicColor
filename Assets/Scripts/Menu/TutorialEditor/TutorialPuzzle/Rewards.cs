using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PuzzleEditor.RouletteEditor;
using UnityEngine;

namespace Menu.TutorialEditor.TutorialPuzzle
{
    public class Rewards : MonoBehaviour
    {
        [SerializeField]
        private float _delay;

        [SerializeField]
        private bool _isImmediately;

        private WaitForSeconds _waitForSeconds;
        private List<Reward> _rewards;

        private void Awake()
        {
            _waitForSeconds = new WaitForSeconds(_delay);
            _rewards = new List<Reward>();

            Reward[] foundRewards = GetComponentsInChildren<Reward>(true);

            foreach (Reward reward in foundRewards)
            {
                _rewards.Add(reward);
            }
        }

        private void OnEnable()
        {
            StartCoroutine(EnableRewardsWithDelay());
        }

        public void Save()
        {
            foreach (Reward reward in _rewards)
            {
                reward.Save();
            }
        }

        public void Appoint(Currency currency, int value)
        {
            List<Reward> matchingRewards = GetSortedList(currency);

            foreach (Reward reward in matchingRewards)
            {
                Show(reward, value);
            }
        }

        private List<Reward> GetSortedList(Currency currency)
        {
            return _rewards
                .Where(reward =>
                    reward.Currency != null && reward.Currency.GetType() == currency.GetType()
                )
                .ToList();
        }

        private void Show(Reward reward, int value)
        {
            reward.SetValue(value);
            reward.Show();
        }

        private IEnumerator EnableRewardsWithDelay()
        {
            yield return _waitForSeconds;

            foreach (Reward reward in _rewards)
            {
                if (reward != null)
                {
                    reward.Show();
                }

                yield return _waitForSeconds;
                yield return _waitForSeconds;
            }

            if (_isImmediately)
            {
                Save();
            }
        }
    }
}