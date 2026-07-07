using UnityEngine;

namespace PuzzleResources.MovingBlocks
{
    [RequireComponent(typeof(Renderer), typeof(ColorableObject))]

    public class TouchColorTransparency : MonoBehaviour
    {
        private Renderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();

            SetAlpha(_renderer.material.color, 0.5f);
        }

        public void SetAlpha(Color color, float alpha)
        {
            color.a = alpha;
            _renderer.material.color = color;
        }
    }
}