using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    [RequireComponent(typeof(Image))]

    public class PuzzleSelector : MonoBehaviour
    {
        private Image _puzzleImage;

        public Sprite Sprite => _puzzleImage.sprite;

        private void Awake()
        {
            _puzzleImage = GetComponent<Image>();

            if (_puzzleImage == null)
            {
                Debug.LogError("Image �� ��������");
                return;
            }
        }

        public void SetSprite(Sprite sprite)
        {
            _puzzleImage.sprite = sprite;
        }
    }
}