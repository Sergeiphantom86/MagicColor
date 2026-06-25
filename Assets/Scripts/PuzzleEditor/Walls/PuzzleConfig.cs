using UnityEngine;

namespace PuzzleEditor.Walls
{
    [CreateAssetMenu(fileName = "PuzzleConfig", menuName = "Puzzles/Puzzle Config")]
    public class PuzzleConfig : ScriptableObject
    {
        [Header("Grid Settings")]
        public int GridSizeX;
        public int GridSizeY;

        [Header("Position Settings")]
        public Vector3 PuzzlePosition;

        [Header("Puzzle Object")]
        public WallsContainer WallsContainerPrefab;
    }
}