using UnityEngine;
using UnityEngine.UI;

namespace Menu.TutorialEditor
{
    public class TutorialPuzzle1 : MonoBehaviour
    {
        private Image _image;

        public Sprite Sprite => _image.sprite;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }
    }
}