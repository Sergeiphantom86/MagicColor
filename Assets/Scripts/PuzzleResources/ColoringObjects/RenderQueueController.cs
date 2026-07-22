using UnityEngine;

namespace PuzzleResources.ColoringObjects
{
    public sealed class RenderQueueController
    {
        private readonly Material _material;

        private int _startRenderQueue;

        public RenderQueueController(Material material)
        {
            _material = material;
        }


        public void SaveStartQueue()
        {
            _startRenderQueue = _material.renderQueue;
        }

        public void Set(int queue)
        {
            _material.renderQueue = queue;
        }

        public void Restore()
        {
            _material.renderQueue = _startRenderQueue;
        }

        public int StartQueue => _startRenderQueue;
    }
}