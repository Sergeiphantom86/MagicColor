using UnityEngine;

namespace PuzzleEditor.MovingBlocks
{
    public interface IDestroyable
    {
        void Destroy(Vector3 waypoint, Vector3 endPoint);
    }
}