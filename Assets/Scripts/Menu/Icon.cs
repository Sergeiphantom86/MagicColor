using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using YG;

namespace Menu
{
    public class Icon : MonoBehaviour
    {
        private TextMeshProUGUI _textMeshProUGUI;

        private void Awake()
        {
            _textMeshProUGUI = GetComponentInChildren<TextMeshProUGUI>();

            SetRank(SetLexel().ToString().NullIfEmpty());
        }

        public void SetRank(string rank)
        {
            if (_textMeshProUGUI == null)
            return;

            _textMeshProUGUI.text = rank;
        }

        private int SetLexel()
        {
            int value = 1;

            value += YG2.saves.MaxReachedQuestIndex;

            return value;
        }
    }
}