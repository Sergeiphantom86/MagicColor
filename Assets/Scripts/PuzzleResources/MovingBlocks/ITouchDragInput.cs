using System;
using UnityEngine;

namespace PuzzleResources.MovingBlocks
{
    public interface ITouchDragInput
    {
        public event Action<Vector2> Touched;

        public event Action<Vector2> TouchDrag;

        public event Action Dropped;

        public bool IsSelected { get; }

        public void OnThrowOff();
    }
}