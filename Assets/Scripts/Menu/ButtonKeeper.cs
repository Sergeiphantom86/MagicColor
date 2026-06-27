using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class ButtonKeeper : MonoBehaviour
    {
        [SerializeField]private Button[] _buttons;
        [SerializeField] private int _currentIndex;

        public Button[] Buttons => _buttons;
    }
}