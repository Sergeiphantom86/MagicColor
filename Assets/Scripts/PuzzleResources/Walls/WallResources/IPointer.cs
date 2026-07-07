using UnityEngine;

namespace PuzzleResources.Walls.WallResources
{
    public interface IPointer
    {
        public Vector3 EndPoint { get; }
        public Vector3 MiddlePoint { get; }
    }
}