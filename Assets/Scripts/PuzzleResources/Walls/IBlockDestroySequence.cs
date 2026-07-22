using System;
using UnityEngine;
using PuzzleResources.ColoringObjects;

namespace PuzzleResources.Walls
{
    public interface IBlockDestroySequence
    {
        public event Action IsTouched;

        public void WaitStart(IColorModifiable colorable, Color color);
    }
}