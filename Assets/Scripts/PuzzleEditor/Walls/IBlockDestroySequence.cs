using System;
using UnityEngine;

namespace PuzzleEditor.Walls
{
    public interface IBlockDestroySequence
    {
        public event Action IsTouched;

        public void WaitStart(IColorable colorable, Color color);
    }
}