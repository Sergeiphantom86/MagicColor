using UnityEngine;

namespace PuzzleResources.Walls
{
    public interface ICollisionProcessor
    {
        void ProcessEnter(Collider other);

        void ProcessExit(Collider other);
    }
}