using UnityEngine;

namespace PuzzleEditor.Walls.WallEditor
{
    public interface IPointer
    {
        public Vector3 EndPoint { get; }
        public Vector3 MiddlePoint { get; }
    }
}