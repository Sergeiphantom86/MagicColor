using UnityEngine;
using UnityEngine.UI;

namespace PuzzleResources.Stars
{
    [RequireComponent(typeof(Image))]

    public class Star : MonoBehaviour
    {
        private Image _image;

        public bool IsActive { get; private set; }
        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        public void SetActive(bool isOn)
        {
            if (_image == null)
            {
                Debug.LogError("Image component is null in Star");
                return;
            }

            _image.enabled = isOn;
            IsActive = isOn;
        }
    }
}