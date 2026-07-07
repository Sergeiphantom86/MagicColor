using System;
using UnityEngine;

namespace PuzzleResources.MovingBlocks
{
    public interface IInputHandler
    {
        public event Action<Vector2> Selected;

        public event Action<Vector2> Moved;

        public event Action Throwed;

        public Vector3 Point { get; }
    }
}