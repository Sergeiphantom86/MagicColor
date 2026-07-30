using UnityEngine;

namespace PuzzleResources.ColoringObjects
{
    public sealed class RendererController
    {
        private readonly Renderer _renderer;

        public RendererController(Renderer renderer)
        {
            _renderer = renderer;
        }

        public void Show()
        {
            _renderer.enabled = true;
        }

        public void Hide()
        {
            _renderer.enabled = false;
        }
    }
}