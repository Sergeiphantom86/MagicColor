using UnityEngine;

namespace PuzzleEditor.PenEditor
{
    public class ColorableWritNode : ColorableObject
    {
        [SerializeField] private Activator _activator;

        private void Start()
        {
            InitializeComponents();
        }

        private void OnEnable()
        {
            _activator.ColorHasChanged += SetColor;
        }

        private void OnDisable()
        {
            _activator.ColorHasChanged -= SetColor;
        }
    }
}