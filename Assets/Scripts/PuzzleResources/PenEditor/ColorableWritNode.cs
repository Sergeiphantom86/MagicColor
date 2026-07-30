using UnityEngine;
using PuzzleResources.ColoringObjects;

namespace PuzzleResources.PenEditor
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
            _activator.OnColorHasChanged += OnSetColor;
        }

        private void OnDisable()
        {
            _activator.OnColorHasChanged -= OnSetColor;
        }
    }
}