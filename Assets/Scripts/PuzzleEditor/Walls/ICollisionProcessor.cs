using UnityEngine;

namespace PuzzleEditor.Walls
{
    public interface ICollisionProcessor
    {
        void ProcessEnter(Collider other);

        void ProcessExit(Collider other);
    }
}