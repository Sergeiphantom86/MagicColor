using System;
using UnityEngine;
using UnityEngine.UI;

namespace Menu.ButtonEditor
{
    public class Exit : MonoBehaviour
    {
        [SerializeField]
        private Button _spinButton;

        private Button _button;

        public event Action<Vector3> Showed;

        private void Awake()
        {
            _button = GetComponent<Button>();

            if (_button == null)
            {
                Debug.LogError("Button == null");
            }
        }

        private void Start()
        {
            _button.onClick.AddListener(OnShow);
        }

        private void OnShow()
        {
            Showed?.Invoke(transform.position);
            _button.interactable = false;

            if (_spinButton != null)
            {
                _spinButton.interactable = false;
            }
        }
    }
}