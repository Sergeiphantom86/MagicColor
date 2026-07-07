using Menu.Tutorials.TutorialPuzzle;
using UnityEngine;
using UnityEngine.UI;

namespace Menu.Tutorials
{
    [RequireComponent(typeof(HandMover))]

    public class HandIndicator : MonoBehaviour
    {
        [SerializeField] private Button _buttonBack;

        private HandMover _handMover;

        private void Awake()
        {
            _handMover = GetComponent<HandMover>();
        }

        private void OnEnable()
        {
            _buttonBack.onClick.AddListener(OnTurnOff);
        }

        private void OnTurnOff()
        {
            _handMover.Destroy();
        }
    }
}