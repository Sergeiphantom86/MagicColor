using TMPro;
using UnityEngine;
using YG;

namespace PuzzleEditor.Counter
{
    public class TextInstaller : MonoBehaviour
    {
        [SerializeField] private Timer _timer;
        [SerializeField] private TextMeshProUGUI _completionTime;
        [SerializeField] private TextMeshProUGUI _countStars;

        private TMP_Text _text;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        private void Start()
        {
            _text.text = $"{_completionTime.text} {_timer.TimerText.text}, {_countStars.text} {YG2.saves.Stars}";
        }
    }
}